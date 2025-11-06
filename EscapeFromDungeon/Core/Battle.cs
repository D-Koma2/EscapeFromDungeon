using EscapeFromDungeon.Behaviors;
using EscapeFromDungeon.Constants;
using EscapeFromDungeon.Models;
using EscapeFromDungeon.Properties;
using EscapeFromDungeon.Services;

namespace EscapeFromDungeon.Core
{
    internal class Battle
    {
        private Player _player;
        private Monster _monster = new Monster("名無し", 1, 1, Weak.None, "Enemy01", new DefaultBehavior());
        private const int _poisonDamageBattle = 3;
        private int _battleTurn = 0;
        private bool _isPlayerDefend = false;

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

        public Battle(Player player) => _player = player;

        public void SetMonster(Monster monster) => _monster = monster;

        public void InitBattleTurn() => _battleTurn = 0;

        public async Task BattleLoopAsync()
        {
            if (_monster.Hp <= 0)
            {
                GameStateManager.Instance.ChangeMode(GameMode.BattleEnd);
                await DrawMessage.ShowAsync($"{_monster.Name}を倒した！");
                GameManager.sePlayer.PlayOnce(Resources.maou_se_8bit27);
                await Task.Delay(500);

                var duration = _monster.Name is Const.demon ? 2 : 16;
                if (CallDrop is not null) await CallDrop.Invoke(600, duration, 8, false);

                SetLabelVisible?.Invoke(true);
                ChangeLblText?.Invoke();
                return;
            }
            if (_player.Hp <= 0 || _player.Limit <= 0)
            {
                GameStateManager.Instance.ChangeMode(GameMode.Gameover);
                await Task.Delay(500);
                return;
            }

            // 戦闘が続いているときはボタンを表示
            if (GameStateManager.Instance.CurrentMode is GameMode.Battle)
            {
                await DrawMessage.ShowAsync(Const.commndMsg);
                SetLabelVisible?.Invoke(true);
            }
        }//BattleLoopAsync

        public async Task PlayerTurnAsync(string command)
        {
            _isPlayerDefend = false;

            switch (command)
            {
                case Const.CommandAtk:
                    var weponName = "";
                    //敵の弱点武器を持っていればダメージアップの処理
                    if (_monster.Weak is not Weak.None)
                    {
                        weakToItemMap.TryGetValue(_monster.Weak, out var weakWepon);
                        weponName = _player.Inventry.Any(item => item.Name == weakWepon) ? weakWepon! : "";
                    }
                    //最強武器を持っていればすべての敵にダメージアップの処理
                    if (_player.Inventry.Any(item => item.Name == Const.superWepon))
                    {
                        weponName = Const.superWepon;
                    }
                    await PlayerAttack(weponName);
                    break;
                case Const.CommandDef:
                    _isPlayerDefend = true;
                    await DrawMessage.ShowAsync($"{_player.Name}は防御の体勢を取った！");
                    break;
                case Const.CommandHeal:
                    if (_player.Inventry.Find(item => item.Name == Const.potion) is not null)
                    {
                        if(_player.Hp == _player.MaxHp)
                        {
                            await DrawMessage.ShowAsync($"{Const.hpFullMsg}");
                            await Task.Delay(400);
                            await BattleLoopAsync();
                            return;
                        }

                        int healPoint = Math.Min(30, _player.MaxHp - _player.Hp);
                        _player.Heal(healPoint);
                        _player.UseItem(Const.potion);
                        GameManager.sePlayer.PlayOnce(Resources.maou_se_magical15);
                        await DrawMessage.ShowAsync($"{_player.Name}は{healPoint}回復した！");
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
                    GameStateManager.Instance.ChangeMode(GameMode.Escaped);
                    GameManager.sePlayer.PlayOnce(Resources.maou_se_battle19);
                    SetLabelVisible?.Invoke(true);
                    if (CallShrink is not null) await CallShrink.Invoke(30, 2);
                    await DrawMessage.ShowAsync($"{_player.Name}は逃げ出した！");
                    _player.Limit--;
                    GameManager.bgmPlayer.PlayLoop(Resources.maou_bgm_8bit04);
                    return;
            }

            await Task.Delay(500);

            if (_monster.Hp > 0) await EnemyTurnAsync();
            else
            {
                _player.Limit--;
                _battleTurn++;
                await BattleLoopAsync();
            }

        }//_playerTurn

        private async Task PlayerAttack(string itemName)
        {
            var behavior = PlayerAttackRegistry.GetBehavior(itemName);
            var action = behavior.DecideAction(_player, _monster, itemName);

            await DrawMessage.ShowAsync(action.Message);
            GameManager.sePlayer.PlayOnce(Resources.maou_se_battle06);
            _monster.TakeDamage(action.Damage);
            if (CallShaker is not null) await CallShaker.Invoke(Target.enemy, Shake.weak, 400, 30);
        }

        private async Task EnemyTurnAsync()
        {
            var action = _monster.behavior.DecideAction(_battleTurn, _monster, _player);

            if (action.SkipDamage)
            {
                await DrawMessage.ShowAsync(action.Message);
            }
            else
            {
                if (_isPlayerDefend)
                {
                    await DrawMessage.ShowAsync($"{_monster.Name}の攻撃！{_player.Name}は防御した！");
                }
                else
                {
                    await DrawMessage.ShowAsync(action.Message);
                    GameManager.sePlayer.PlayOnce(Resources.maou_se_8bit22);
                    _player.TakeDamage(action.Damage);
                    if (CallShaker is not null) await CallShaker.Invoke(Target.player, action.shakeType, 400, 30);
                }
            }

            await Task.Delay(500);

            if (_player.Hp <= 0)
            {
                await BattleLoopAsync();
                return;
            }

            await IsStatusPoison();

            _player.Limit--;
            _battleTurn++;
            await BattleLoopAsync();
        }

        private async Task IsStatusPoison()
        {
            if (_player.Status is Status.Poison)
            {
                if (_player.GetItemCount(Const.curePoison) > 0)
                {
                    await DrawMessage.ShowAsync($"{_player.Name}は{Const.curePoison}を使った！");
                    _player.HealStatus();
                }
                else
                {
                    _player.TakeDamage(_poisonDamageBattle);
                    await DrawMessage.ShowAsync($"{_player.Name}は毒で{_poisonDamageBattle}のダメージ！");
                    if (CallShaker is not null) await CallShaker.Invoke(Target.player, Shake.normal, 400, 30);
                }
                await Task.Delay(500);
            }
        }

    }//class Battle
}//namespace EscapeFromDungeon
