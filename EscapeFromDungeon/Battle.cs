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
        public int BattleTurn { get; set; } = 0;

        private bool _isDefending;

        public Action<bool>? SetButtonEnabled;
        public Action<bool>? SetMonsterVisible;
        public Action? ChangeLblText;

        public Battle(Player player, Message message)
        {
            this.Player = player;
            this.message = message;
            Monster = new Monster("仮モンスター", 1, 1, Weak.None);
        }

        public async Task BattleLoopAsync()
        {
            if (Monster.Hp <= 0)
            {
                GameManager.gameMode = GameMode.BattleEnd;
                await message.ShowAsync($"{Monster.Name}を倒した！");
                await Task.Delay(500);
                SetMonsterVisible?.Invoke(false);
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
                await message.ShowAsync($"コマンド？");
                SetButtonEnabled?.Invoke(true);
            }
        }//BattleLoopAsync

        public async Task PlayerTurnAsync(string command)
        {
            Player.Limit--;
            _isDefending = false;

            switch (command)
            {
                case "Attack":

                    //ここで敵の弱点のアイテムを持っていればダメージアップの処理
                    if (Monster.Weak != Weak.None)
                    {
                        string itemName = Monster.Weak switch
                        {
                            Weak.Fire => "炎の剣",
                            Weak.Ice => "氷の剣",
                            Weak.Heavy => "おおきづち",
                            Weak.Holy => "聖なる剣",
                            _ => ""
                        };
                        if (Player.Inventry.Any(item => item.Name == itemName))
                        {
                            int extraDamage = Player.Attack * 2;
                            Monster.Hp -= extraDamage;
                            await message.ShowAsync($"{Player.Name}は{itemName}で攻撃！{Monster.Name}に {extraDamage} ダメージ！");
                        }
                        else
                        {
                            Monster.Hp -= Player.Attack;
                            await message.ShowAsync($"{Player.Name}の攻撃！{Monster.Name}に {Player.Attack} ダメージ！");
                        }
                    }
                    else
                    {
                        Monster.Hp -= Player.Attack;
                        await message.ShowAsync($"{Player.Name}の攻撃！{Monster.Name}に {Player.Attack} ダメージ！");
                    }
                    break;
                case "Defence":
                    _isDefending = true;
                    await message.ShowAsync($"{Player.Name}は防御の体勢を取った！");
                    break;
                case "Heal":
                    if (Player.Inventry.Find(item => item.Name == "ポーション") != null)//仮 HPMAｘのときは押せないようにする予定
                    {
                        int point = 30;
                        point = Math.Min(point, Player.MaxHp - Player.Hp);
                        Player.Hp += point;
                        var item = Player.Inventry.Find(item => item.Name == "ポーション");
                        if (item != null) Player.Inventry.Remove(item);
                        await message.ShowAsync($"{Player.Name}は{point}回復した！");
                    }
                    else
                    {
                        await message.ShowAsync("ポーションがない！");//仮 所持してないときはボタンを押せないようにする予定
                    }
                    break;
                case "Escape":
                    GameManager.gameMode = GameMode.Escaped;
                    SetMonsterVisible?.Invoke(false);
                    SetButtonEnabled?.Invoke(true);
                    await Task.Delay(500);
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

                if (Monster.Name == "デーモン")
                {
                    if (BattleTurn % 5 == 4)
                    {
                        damage *= 3;
                        Player.Hp -= damage;
                        await message.ShowAsync($"{Monster.Name}の強力な攻撃！{Player.Name}は {damage} 大ダメージ！");
                    }
                    else if (BattleTurn % 5 == 3)
                    {
                        await message.ShowAsync($"{Monster.Name}は力をためている！");
                    }
                    else
                    {
                        Player.Hp -= damage;
                        await message.ShowAsync($"{Monster.Name}の攻撃！{Player.Name}は {damage} ダメージ！");
                    }
                }
                else if (Monster.Name == "炎スライム" || Monster.Name == "氷スライム" || Monster.Name == "雷スライム")
                {
                    if (BattleTurn % 4 == 3)
                    {
                        damage *= 2;
                        Player.Hp -= damage;
                        await message.ShowAsync($"{Monster.Name}の強力な攻撃！{Player.Name}は {damage} 大ダメージ！");
                    }
                    else if (BattleTurn % 4 == 2)
                    {
                        await message.ShowAsync($"{Monster.Name}は力をためている！");
                    }
                    else
                    {
                        Player.Hp -= damage;
                        await message.ShowAsync($"{Monster.Name}の攻撃！{Player.Name}は {damage} ダメージ！");
                    }
                }
                else
                {
                    Player.Hp -= damage;
                    await message.ShowAsync($"{Monster.Name}の攻撃！{Player.Name}は {damage} ダメージ！");
                }
            }

            await Task.Delay(500);
            BattleTurn++;
            await BattleLoopAsync();
        }//EnemyTurn

    }//class Battle
}//namespace EscapeFromDungeon
