using EscapeFromDungeon.Constants;
using EscapeFromDungeon.Models;
using EscapeFromDungeon.Properties;
using EscapeFromDungeon.Services;

namespace EscapeFromDungeon.Core
{
    internal class GameManager
    {
        private const int _ViewShrinkInterval = 33;
        private const int _DamageFloorValue = 3;
        private const int _PoisonDamageValue = 1;

        private Point _eventPos = Point.Empty;
        private Point _prePos = Point.Empty;

        public Player Player { get; private set; }
        public Battle Battle { get; private set; }

        public static MusicPlayer bgmPlayer = new MusicPlayer();
        public static MusicPlayer sePlayer = new MusicPlayer();

        public Action? SetMapPos;
        public Action? SetLabelBaseCol;
        public Action? ChangeLblText;

        public Func<string, Keys, Task>? MoveKeyPressed;
        public Action<string>? ItemKeyPressed;

        public Action<Image>? SetMonsterImg;
        public Action<FadeForm.FadeDir>? StartFade;
        public Func<int, int, int, bool, Task>? CallDrop;

        private const string _playerName = "あなた";
        private const int _playerHp = 100;
        private const int _playerAttack = 10;

        public readonly int limitMax = 999;

        public GameManager()
        {
            Map.ReadFromCsv(Const.mapCsv);//最初に実行する事!
            EventData.ReadFromCsv(Const.eventCsv);
            MonsterData.ReadFromCsv(Const.monsterCsv);
            ItemData.ReadFromCsv(Const.itemCsv);
            DrawMessage.timerSetup();
            Player = new Player(_playerName, _playerHp, _playerAttack, limitMax);
            Battle = new Battle(Player);
        }

        public void KeyInput(Keys keyCode)
        {
            if (GameStateManager.Instance.CurrentMode == GameMode.Explore)
            {
                if (keyCode == Keys.Up || keyCode == Keys.Down || keyCode == Keys.Left || keyCode == Keys.Right)
                {
                    // メッセージ表示中はメッセージ処理優先
                    if (DrawMessage.IsMessageShowing)
                    {
                        DrawMessage.InputKey();
                        return;
                    }

                    Move(keyCode);
                }
                else if (keyCode == Keys.P)
                {
                    ItemKeyPressed?.Invoke(Const.potion);
                }
                else if (keyCode == Keys.O)
                {
                    ItemKeyPressed?.Invoke(Const.curePoison);
                }
                else if (keyCode == Keys.I)
                {
                    ItemKeyPressed?.Invoke(Const.torch);
                }
                //else if (keyCode == Keys.V)//デバッグ用
                //{
                //    if (Map.IsVisionEnabled) Map.SetIsVisionEnable(false);
                //    else Map.SetIsVisionEnable(true);
                //}
            }
            else if (GameStateManager.Instance.CurrentMode == GameMode.Battle)
            {
                if (keyCode == Keys.Up)
                {
                    MoveKeyPressed?.Invoke(Const.CommandAtk, Keys.Up);
                }
                else if (keyCode == Keys.Down)
                {
                    MoveKeyPressed?.Invoke(Const.CommandEsc, Keys.Down);
                }
                else if (keyCode == Keys.Left)
                {
                    MoveKeyPressed?.Invoke(Const.CommandDef, Keys.Left);
                }
                else if (keyCode == Keys.Right)
                {
                    MoveKeyPressed?.Invoke(Const.CommandHeal, Keys.Right);
                }
            }
        }

        public void Init()
        {
            Player.Init(_playerHp, limitMax);
            _eventPos = Point.Empty;
        }

        public async Task BattleCheckAsync()
        {
            if (GameStateManager.Instance.CurrentMode == GameMode.Escaped)
            {
                Form1.isBattleInputLocked = true;
                Form1.battleInputUnlockTime = DateTime.Now.AddSeconds(2.5); // 指定秒間キー入力をロック
                Map.PlayerPos = _prePos;
                SetMapPos?.Invoke();
                await Task.Delay(500);
                GameStateManager.Instance.ChangeMode(GameMode.Explore);
                ChangeLblText?.Invoke();
            }

            if (GameStateManager.Instance.CurrentMode == GameMode.BattleEnd)
            {
                if (Player.Hp > 0)
                {
                    await DrawMessage.ShowAsync($"{Player.Name}は勝利した!");
                    await Task.Delay(500);
                    // モンスターを倒したらイベントを消去
                    Map.DeleteEvent(_eventPos.X, _eventPos.Y);
                    //マップ上の敵シンボルを消す
                    Map.DelEnemySimbolDraw(_eventPos.X, _eventPos.Y);
                    _eventPos = Point.Empty;
                    GameStateManager.Instance.ChangeMode(GameMode.Explore);
                    ChangeLblText?.Invoke();
                    bgmPlayer.PlayLoop(Resources.maou_bgm_8bit04);
                }
            }

            if (GameStateManager.Instance.CurrentMode == GameMode.Gameover) Gameover();
        }//BattleCheck

        private async void Move(Keys keyCode)
        {
            Point dir = Point.Empty;
            var playerDir = Player.Direction.Up;

            if (keyCode == Keys.Up)
            {
                dir = new Point(0, -1);
                playerDir = Player.Direction.Up;
            }
            else if (keyCode == Keys.Down)
            {
                dir = new Point(0, 1);
                playerDir = Player.Direction.Down;
            }
            else if (keyCode == Keys.Left)
            {
                dir = new Point(-1, 0);
                playerDir = Player.Direction.Left;
            }
            else if (keyCode == Keys.Right)
            {
                dir = new Point(1, 0);
                playerDir = Player.Direction.Right;
            }

            _prePos = Map.PlayerPos; // 移動前の位置を保存

            Player.SetDirectionImage(playerDir);
            Point newPos = new Point(Map.PlayerPos.X + dir.X, Map.PlayerPos.Y + dir.Y);

            // 移動可能かチェック
            if (Map.CanMoveTo(newPos.X, newPos.Y))
            {
                _eventPos = newPos; // モンスターイベント位置を保存

                Map.PlayerPos = newPos;
                SetMapPos?.Invoke();

                Event? evt = await CheckEvent(newPos.X, newPos.Y);

                if (evt == null) DrawMessage.Init(); // メッセージリセット

                if (evt != null && evt.EventType == EventType.Encount) return;

                DamageCheck(newPos.X, newPos.Y);
                TurnCheck();
            }
        }//Move 

        private void DamageCheck(int x, int y)
        {
            var isDamaged = false;
            // ダメージ床
            if (Map.WalkMap[x, y] == 3)
            {
                Player.TakeDamage(_DamageFloorValue);
                isDamaged = true;
            }
            if (Player.Status == Status.Poison)
            {
                Player.TakeDamage(_PoisonDamageValue);
                isDamaged = true;
            }

            if (isDamaged)
            {
                sePlayer.PlayOnce(Resources.maou_se_8bit22);
            }
            else
            {
                sePlayer.PlayOnce(Resources.maou_se_sound_footstep02);
            }
        }

        private void TurnCheck()
        {
            Player.Limit--;

            if (Player.Limit <= 0 || Player.Hp <= 0)
            {
                GameStateManager.Instance.ChangeMode(GameMode.Gameover); Gameover();
            }

            var invertCount = limitMax - Player.Limit;
            if (invertCount % _ViewShrinkInterval == 0) Map.AddViewRadius(-1);
        }

        private async Task<Event?> CheckEvent(int x, int y)
        {
            var eventId = Map.EventMap[x, y];

            if (eventId == null) return null; // イベントなし

            Event evt = EventData.Dict[eventId];

            if (!EventData.Dict.ContainsKey(eventId)) return null;

            switch (evt.EventType)
            {
                case EventType.Message:
                    await DrawMessage.ShowAsync(evt.Word);
                    await Task.Delay(500);
                    break;
                case EventType.Hint:
                    await DrawMessage.ShowAsync(evt.Word);
                    await Task.Delay(500);
                    break;
                case EventType.ItemGet:
                    ItemGetEvent(evt);
                    break;
                case EventType.Heal:
                    HealEvent(evt);
                    break;
                case EventType.Trap:
                    TrapEvent(evt);
                    break;
                case EventType.Encount:
                    EncounterEventAsync(evt);
                    break;
                case EventType.GameClear:
                    bgmPlayer.PlayLoop(Resources.maou_bgm_8bit06);
                    await DrawMessage.ShowAsync(evt.Word);
                    SetLabelBaseCol?.Invoke();
                    await Task.Delay(500);
                    GameStateManager.Instance.ChangeMode(GameMode.GameClear);
                    if (StartFade is not null) StartFade(FadeForm.FadeDir.FadeIn);
                    break;
                default:
                    break;
            }

            // ヒント以外は一度きり,バトルは逃げた場合残すのでここでは消去しない
            if (evt.EventType != EventType.Hint && evt.EventType != EventType.Encount)
            {
                Map.DeleteEvent(x, y); // イベントを消去
            }

            return evt;

        }//CheckEvent   

        private async void EncounterEventAsync(Event evt)
        {
            bgmPlayer.PlayLoop(Resources.maou_bgm_8bit08);

            Form1.isBattleInputLocked = true;
            Form1.battleInputUnlockTime = DateTime.Now.AddSeconds(3.0); // 指定秒間キー入力をロック

            GameStateManager.Instance.ChangeMode(GameMode.Battle);
            var mon = MonsterData.Dict[evt.Word];
            Battle.SetMonster(new Monster(mon.Name, mon.Hp, mon.Attack, mon.Weak, mon.ImageName, mon.behavior));

            //モンスターイメージを変更
            Image? img = Resources.ResourceManager.GetObject(mon.ImageName) as Image;
            if (img != null) SetMonsterImg?.Invoke(img);

            Battle.InitBattleTurn();

            if (CallDrop != null) await CallDrop.Invoke(600, 10, 4, true);
            ChangeLblText?.Invoke();

            await DrawMessage.ShowAsync($"{mon.Name}が現れた！");
            await Task.Delay(500);
            await DrawMessage.ShowAsync(Const.commndMsg);
            await Task.Delay(400);
            Battle.SetLabelVisible?.Invoke(true);
        }

        private async void ItemGetEvent(Event evt)
        {
            var item = ItemData.Dict[evt.Word];
            var name = item.Name;
            var dsc = item.Description;
            Player.GetItem(name, dsc);
            await DrawMessage.ShowAsync($"アイテム「{evt.Word}」を取得！");
            sePlayer.PlayOnce(Resources.maou_se_onepoint09);
            await Task.Delay(500);
        }

        private async void HealEvent(Event evt)
        {
            await DrawMessage.ShowAsync(evt.Word);
            sePlayer.PlayOnce(Resources.maou_se_magical15);
            await Task.Delay(500);

            switch (evt.Id)
            {
                case "L0":
                    Player.Heal(20);
                    break;
                case "L1":
                    Player.Heal(50);
                    break;
                case "L2":
                    Player.Heal(100);
                    break;
                case "L3":
                    Player.HealStatus();
                    break;
                default:
                    break;
            }
        }

        private async void TrapEvent(Event evt)
        {
            await DrawMessage.ShowAsync(evt.Word);
            sePlayer.PlayOnce(Resources.maou_se_battle12);
            await Task.Delay(500);

            switch (evt.Id)
            {
                case "T0":
                    Player.TakeDamage(5);
                    break;
                case "T1":
                    Player.TakeDamage(20);
                    break;
                case "T2":
                    Player.TakeDamage(40);
                    break;
                case "T3":
                    Player.TakePoison();
                    break;
                default:
                    break;
            }

            TurnCheck();
        }
        private async void Gameover()
        {
            GameManager.bgmPlayer.PlayLoop(Properties.Resources.maou_bgm_8bit20);
            await DrawMessage.ShowAsync($"{Player.Name}は力尽きた...");
            SetLabelBaseCol?.Invoke();
            await Task.Delay(500);
            if (StartFade is not null) StartFade(FadeForm.FadeDir.FadeIn);
        }

        public void PlayerVisible(PictureBox playerImage)
        {
            playerImage.Visible = Map.WalkMap[Map.PlayerPos.X, Map.PlayerPos.Y] != 2 ? true : false;
        }

    }//class
}//namespace
