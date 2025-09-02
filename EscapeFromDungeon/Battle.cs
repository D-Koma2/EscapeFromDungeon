using EscapeFromDungeon.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    internal class Battle
    {
        public Monster Monster { get; set; }
        public Player Player { get; set; }
        public Message message { get; set; }

        private int _battleTurn = 0;

        private bool _isDefending;

        public Action<bool>? SetButtonEnabled;
        public Action<bool>? SetMonsterVisible;
        public Action? ChangeLblText;
        public Func<int, int, int, int, Task>? CallShaker;
        public Func<int, int, int, bool, Task>? CallDrop;
        public Func<int, int, Task>? CallShrink;

        public Battle(Player player, Message message)
        {
            this.Player = player;
            this.message = message;
            Monster = new Monster("仮モンスター", 1, 1, Weak.None, "monster01");
        }

        public void RestBattleTurn() => _battleTurn = 0;

        public async Task BattleLoopAsync()
        {
            if (Monster.Hp <= 0)
            {
                GameManager.gameMode = GameMode.BattleEnd;
                await message.ShowAsync($"{Monster.Name}を倒した！");
                await Task.Delay(500);
                if (Monster.Name == Const.demon)
                {
                    if (CallDrop != null) await CallDrop.Invoke(600, 2, 8, false);
                }
                else
                {
                    if (CallDrop != null) await CallDrop.Invoke(600, 16, 8, false);
                }
                SetButtonEnabled?.Invoke(true);
                ChangeLblText?.Invoke();
                return;
            }
            if (Player.Hp <= 0 || Player.Limit <= 0)
            {
                GameManager.gameMode = GameMode.Gameover;
                await message.ShowAsync($"{Player.Name}は力尽きた...");
                await Task.Delay(500);
                SetMonsterVisible?.Invoke(false);
                return;
            }

            // 戦闘が続いているときだけボタンを表示
            if (GameManager.gameMode == GameMode.Battle)
            {
                await message.ShowAsync(Const.commndMsg);
                SetButtonEnabled?.Invoke(true);
            }
        }//BattleLoopAsync

        public async Task PlayerTurnAsync(string command)
        {
            Player.Limit--;
            _isDefending = false;

            switch (command)
            {
                case Const.CommandAtk:

                    //敵の弱点のアイテムを持っていればダメージアップの処理
                    if (Monster.Weak != Weak.None)
                    {
                        string itemName = Monster.Weak switch
                        {
                            Weak.Fire => Const.fireWepon,
                            Weak.Ice => Const.iceWepon,
                            Weak.Heavy => Const.hitWepon,
                            Weak.Holy => Const.holyWepon,
                            _ => ""
                        };
                        if (Player.Inventry.Any(item => item.Name == itemName))
                        {
                            int extraDamage = Player.Attack * 3;
                            Monster.TakeDamage(extraDamage);
                            await message.ShowAsync($"{Player.Name}は{itemName}で攻撃！{Monster.Name}に {extraDamage} 大ダメージ！");
                            if (CallShaker != null) await CallShaker.Invoke(2, 2, 400, 30);
                            break;
                        }
                    }

                    Monster.TakeDamage(Player.Attack);
                    await message.ShowAsync($"{Player.Name}の攻撃！{Monster.Name}に {Player.Attack} ダメージ！");
                    if (CallShaker != null) await CallShaker.Invoke(2, 1, 400, 30);
                    break;
                case Const.CommandDef:
                    _isDefending = true;
                    await message.ShowAsync($"{Player.Name}は防御の体勢を取った！");
                    break;
                case Const.CommandHeal:
                    if (Player.Inventry.Find(item => item.Name == Const.potion) != null)
                    {
                        int point = 30;
                        point = Math.Min(point, Player.MaxHp - Player.Hp);
                        Player.Heal(point);
                        Player.UseItem(Const.potion);
                        await message.ShowAsync($"{Player.Name}は{point}回復した！");
                    }
                    else
                    {
                        await message.ShowAsync($"{Const.potion}を持っていなかった！");
                    }
                    break;
                case Const.CommandEsc:
                    GameManager.gameMode = GameMode.Escaped;
                    SetButtonEnabled?.Invoke(true);
                    if (CallShrink != null) await CallShrink.Invoke(30, 2);
                    await message.ShowAsync($"{Player.Name}は逃げ出した！");
                    return;
            }

            await Task.Delay(500);
            if (Monster.Hp > 0) await EnemyTurnAsync();
            else await BattleLoopAsync();
        }//PlayerTurn

        private async Task EnemyTurnAsync()
        {
            if (_isDefending)
            {
                await message.ShowAsync($"{Monster.Name}の攻撃！{Player.Name}は防御した！");
            }
            else
            {
                int damage = Monster.Attack;

                if (Monster.Name == Const.demon)
                {
                    if (_battleTurn % 5 == 4)
                    {
                        damage *= 3;
                        Player.TakeDamage(damage);
                        await message.ShowAsync($"{Monster.Name}の強力な攻撃！{Player.Name}は {damage} 大ダメージ！");
                        if (CallShaker != null) await CallShaker.Invoke(1, 1, 400, 30);
                    }
                    else if (_battleTurn % 5 == 3)
                    {
                        await message.ShowAsync($"{Monster.Name}は力をためている！");
                    }
                    else
                    {
                        Player.TakeDamage(damage);
                        await message.ShowAsync($"{Monster.Name}の攻撃！{Player.Name}は {damage} ダメージ！");
                        if (CallShaker != null) await CallShaker.Invoke(1, 1, 400, 30);
                    }
                }
                else if (Monster.Name == Const.fireSlime || Monster.Name == Const.iceSlime || Monster.Name == Const.thunderSlime)
                {
                    if (_battleTurn % 4 == 3)
                    {
                        damage *= 2;
                        Player.TakeDamage(damage);
                        await message.ShowAsync($"{Monster.Name}の強力な攻撃！{Player.Name}は {damage} 大ダメージ！");
                        if (CallShaker != null) await CallShaker.Invoke(1, 2, 400, 30);
                    }
                    else if (_battleTurn % 4 == 2)
                    {
                        await message.ShowAsync($"{Monster.Name}は力をためている！");
                    }
                    else
                    {
                        Player.TakeDamage(damage);
                        await message.ShowAsync($"{Monster.Name}の攻撃！{Player.Name}は {damage} ダメージ！");
                        if (CallShaker != null) await CallShaker.Invoke(1, 1, 400, 30);
                    }
                }
                else
                {
                    Player.TakeDamage(damage);
                    await message.ShowAsync($"{Monster.Name}の攻撃！{Player.Name}は {damage} ダメージ！");
                    if (CallShaker != null) await CallShaker.Invoke(1, 1, 400, 30);
                }
            }

            await Task.Delay(500);
            _battleTurn++;
            if (Player.Status == Status.Poison)
            {
                if (Player.GetItemCount(Const.curePoison) > 0)
                {
                    await message.ShowAsync($"{Player.Name}は{Const.curePoison}を使った！");
                    Player.HealStatus();
                }
                else
                {
                    int poisonDamage = 3;
                    Player.TakeDamage(poisonDamage);
                    await message.ShowAsync($"{Player.Name}は毒のダメージを受けた！{poisonDamage}ダメージ！");
                    if (CallShaker != null) await CallShaker.Invoke(1, 1, 400, 30);
                }
                await Task.Delay(500);
            }
            await BattleLoopAsync();
        }//EnemyTurn

    }//class Battle
}//namespace EscapeFromDungeon
