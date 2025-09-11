using EscapeFromDungeon.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    internal class Battle
    {
        public Monster Monster { get; set; }
        public Player Player { get; set; }

        private int _battleTurn = 0;
        private bool _isDefending = false;

        public Action<bool>? SetLabelVisible;
        public Action<bool>? SetMonsterVisible;
        public Action? ChangeLblText;
        public Func<Target, Shake, int, int, Task>? CallShaker;
        public Func<int, int, int, bool, Task>? CallDrop;
        public Func<int, int, Task>? CallShrink;

        private static readonly Dictionary<Weak, string> weakToItemMap = new()
        {
            { Weak.Fire, Const.fireWepon },
            { Weak.Ice, Const.iceWepon },
            { Weak.Thunder, Const.thunderWepon },
            { Weak.Heavy, Const.heavyWepon },
            { Weak.Holy, Const.holyWepon }
        };

        public Battle(Player player)
        {
            this.Player = player;
            Monster = new Monster("仮スライム君", 1, 1, Weak.None, "Enemy01");
        }

        public void InitBattleTurn() => _battleTurn = 0;

        public async Task BattleLoopAsync()
        {
            if (Monster.Hp <= 0)
            {
                GameManager.gameMode = GameMode.BattleEnd;
                await Message.ShowAsync($"{Monster.Name}を倒した！");
                await Task.Delay(500);
                var duration = (Monster.Name == Const.demon) ? 2 : 16;
                if (CallDrop != null) await CallDrop.Invoke(600, duration, 8, false);
                SetLabelVisible?.Invoke(true);
                ChangeLblText?.Invoke();
                return;
            }
            if (Player.Hp <= 0 || Player.Limit <= 0)
            {
                GameManager.gameMode = GameMode.Gameover;
                await Task.Delay(500);
                return;
            }

            // 戦闘が続いているときはボタンを表示
            if (GameManager.gameMode == GameMode.Battle)
            {
                await Message.ShowAsync(Const.commndMsg);
                SetLabelVisible?.Invoke(true);
            }
        }//BattleLoopAsync

        public async Task PlayerTurnAsync(string command)
        {
            _isDefending = false;

            switch (command)
            {
                case Const.CommandAtk:

                    string itemName = "";

                    //敵の弱点武器を持っていればダメージアップの処理
                    if (Monster.Weak != Weak.None)
                    {
                        weakToItemMap.TryGetValue(Monster.Weak, out var weakWepon);
                        itemName = Player.Inventry.Any(item => item.Name == weakWepon) ? weakWepon! : "";
                    }

                    //最強武器を持っていればすべての敵にダメージアップの処理
                    if (Player.Inventry.Find(item => item.Name == Const.superWepon) != null)
                    {
                        itemName = Const.superWepon;
                    }
                    
                    await PlayerWeakAttack(itemName);
                    break;
                case Const.CommandDef:
                    _isDefending = true;
                    await Message.ShowAsync($"{Player.Name}は防御の体勢を取った！");
                    break;
                case Const.CommandHeal:
                    if (Player.Inventry.Find(item => item.Name == Const.potion) != null)
                    {
                        int point = 30;
                        point = Math.Min(point, Player.MaxHp - Player.Hp);
                        Player.Heal(point);
                        Player.UseItem(Const.potion);
                        await Message.ShowAsync($"{Player.Name}は{point}回復した！");
                        break;
                    }
                    else
                    {
                        await Message.ShowAsync($"{Const.potion}を持っていない！");
                        await Task.Delay(400);
                        await BattleLoopAsync();
                        return;
                    }
                case Const.CommandEsc:
                    GameManager.gameMode = GameMode.Escaped;
                    SetLabelVisible?.Invoke(true);
                    if (CallShrink != null) await CallShrink.Invoke(30, 2);
                    await Message.ShowAsync($"{Player.Name}は逃げ出した！");
                    Player.Limit--;
                    return;
            }

            await Task.Delay(500);

            if (Monster.Hp > 0) await EnemyTurnAsync();
            else
            {
                Player.Limit--;
                _battleTurn++;
                await BattleLoopAsync();
            }

        }//PlayerTurn

        private async Task PlayerWeakAttack(string itemName)
        {
            var behavior = PlayerAttackRegistry.GetBehavior(itemName);
            var action = behavior.DecideAction(Player, Monster, itemName);

            await Message.ShowAsync(action.Message);
            Monster.TakeDamage(action.Damage);
            if (CallShaker != null) await CallShaker.Invoke(Target.enemy, Shake.weak, 400, 30);
        }

        private async Task EnemyTurnAsync()
        {
            var behavior = MonsterBehaviorRegistry.GetBehavior(Monster.Name);
            var action = behavior.DecideAction(_battleTurn, Monster, Player);

            if (action.SkipDamage)
            {
                await Message.ShowAsync(action.Message);
            }
            else
            {
                if (_isDefending)
                {
                    await Message.ShowAsync($"{Monster.Name}の攻撃！{Player.Name}は防御した！");
                }
                else
                {
                    await Message.ShowAsync(action.Message);
                    Player.TakeDamage(action.Damage);
                    if (CallShaker != null) await CallShaker.Invoke(Target.player, action.shakeType, 400, 30);
                }
            }

            await Task.Delay(500);
            await IsStatusPoison();

            Player.Limit--;
            _battleTurn++;
            await BattleLoopAsync();
        }

        private async Task IsStatusPoison()
        {
            if (Player.Status == Status.Poison)
            {
                if (Player.GetItemCount(Const.curePoison) > 0)
                {
                    await Message.ShowAsync($"{Player.Name}は{Const.curePoison}を使った！");
                    Player.HealStatus();
                }
                else
                {
                    int poisonDamage = 3;
                    Player.TakeDamage(poisonDamage);
                    await Message.ShowAsync($"{Player.Name}は毒で{poisonDamage}のダメージ！");
                    if (CallShaker != null) await CallShaker.Invoke(Target.player, Shake.normal, 400, 30);
                }
                await Task.Delay(500);
            }
        }

    }//class Battle
}//namespace EscapeFromDungeon
