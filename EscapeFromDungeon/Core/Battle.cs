using EscapeFromDungeon.Behaviors;
using EscapeFromDungeon.Constants;
using EscapeFromDungeon.Models;
using EscapeFromDungeon.Properties;
using EscapeFromDungeon.Services;

namespace EscapeFromDungeon.Core
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
            Player = player;
            Monster = new Monster("仮スライム君", 1, 1, Weak.None, "Enemy01", new DefaultBehavior());
        }

        public void InitBattleTurn() => _battleTurn = 0;

        public async Task BattleLoopAsync()
        {
            if (Monster.Hp <= 0)
            {
                GameManager.gameMode = GameMode.BattleEnd;
                await DrawMessage.ShowAsync($"{Monster.Name}を倒した！");
                GameManager.sePlayer.PlayOnce(Resources.maou_se_8bit27);
                await Task.Delay(500);
                var duration = Monster.Name == Const.demon ? 2 : 16;
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
                await DrawMessage.ShowAsync(Const.commndMsg);
                SetLabelVisible?.Invoke(true);
            }
        }//BattleLoopAsync

        public async Task PlayerTurnAsync(string command)
        {
            _isDefending = false;

            switch (command)
            {
                case Const.CommandAtk:
                    var weponName = "";
                    //敵の弱点武器を持っていればダメージアップの処理
                    if (Monster.Weak != Weak.None)
                    {
                        weakToItemMap.TryGetValue(Monster.Weak, out var weakWepon);
                        weponName = Player.Inventry.Any(item => item.Name == weakWepon) ? weakWepon! : "";
                    }
                    //最強武器を持っていればすべての敵にダメージアップの処理
                    if (Player.Inventry.Any(item => item.Name == Const.superWepon))
                    {
                        weponName = Const.superWepon;
                    }
                    await PlayerAttack(weponName);
                    break;
                case Const.CommandDef:
                    _isDefending = true;
                    await DrawMessage.ShowAsync($"{Player.Name}は防御の体勢を取った！");
                    break;
                case Const.CommandHeal:
                    if (Player.Inventry.Find(item => item.Name == Const.potion) != null)
                    {
                        int healPoint = Math.Min(30, Player.MaxHp - Player.Hp);
                        Player.Heal(healPoint);
                        Player.UseItem(Const.potion);
                        GameManager.sePlayer.PlayOnce(Resources.maou_se_magical15);
                        await DrawMessage.ShowAsync($"{Player.Name}は{healPoint}回復した！");
                        break;
                    }
                    else
                    {
                        await DrawMessage.ShowAsync($"{Const.potion}を持っていない！");
                        await Task.Delay(400);
                        await BattleLoopAsync();
                        return;
                    }
                case Const.CommandEsc:
                    GameManager.gameMode = GameMode.Escaped;
                    GameManager.sePlayer.PlayOnce(Properties.Resources.maou_se_battle19);
                    SetLabelVisible?.Invoke(true);
                    if (CallShrink != null) await CallShrink.Invoke(30, 2);
                    await DrawMessage.ShowAsync($"{Player.Name}は逃げ出した！");
                    Player.Limit--;
                    GameManager.bgmPlayer.PlayLoop(Properties.Resources.maou_bgm_8bit04);
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

        private async Task PlayerAttack(string itemName)
        {
            var behavior = PlayerAttackRegistry.GetBehavior(itemName);
            var action = behavior.DecideAction(Player, Monster, itemName);

            await DrawMessage.ShowAsync(action.Message);
            GameManager.sePlayer.PlayOnce(Resources.maou_se_battle06);
            Monster.TakeDamage(action.Damage);
            if (CallShaker != null) await CallShaker.Invoke(Target.enemy, Shake.weak, 400, 30);
        }

        private async Task EnemyTurnAsync()
        {
            var action = Monster.behavior.DecideAction(_battleTurn, Monster, Player);

            if (action.SkipDamage)
            {
                await DrawMessage.ShowAsync(action.Message);
            }
            else
            {
                if (_isDefending)
                {
                    await DrawMessage.ShowAsync($"{Monster.Name}の攻撃！{Player.Name}は防御した！");
                }
                else
                {
                    await DrawMessage.ShowAsync(action.Message);
                    GameManager.sePlayer.PlayOnce(Resources.maou_se_8bit22);
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
                    await DrawMessage.ShowAsync($"{Player.Name}は{Const.curePoison}を使った！");
                    Player.HealStatus();
                }
                else
                {
                    int poisonDamage = 3;
                    Player.TakeDamage(poisonDamage);
                    await DrawMessage.ShowAsync($"{Player.Name}は毒で{poisonDamage}のダメージ！");
                    if (CallShaker != null) await CallShaker.Invoke(Target.player, Shake.normal, 400, 30);
                }
                await Task.Delay(500);
            }
        }

    }//class Battle
}//namespace EscapeFromDungeon
