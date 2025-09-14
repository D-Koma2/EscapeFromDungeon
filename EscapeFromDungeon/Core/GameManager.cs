using EscapeFromDungeon.Constants;
using EscapeFromDungeon.Models;
using EscapeFromDungeon.Properties;
using EscapeFromDungeon.Services;

namespace EscapeFromDungeon.Core
{
    public enum GameMode
    {
        Title,
        Explore,
        Battle,
        Escaped,
        BattleEnd,
        Gameover,
        GameClear,
        Reset
    }

    internal class GameManager
    {
        public static GameMode gameMode { get; set; } = GameMode.Title;
        // true: 視界制限あり、false: 全体表示デバッグ用
        public static bool IsVisionEnabled { get; set; } = true;

        private const int ViewShrinkInterval = 33;
        private const int DamageFloorValue = 3;
        private const int PoisonDamageValue = 1;

        private const string _playerName = "あなた";
        private const int _playerHp = 100;
        private const int _playerAttack = 10;
        public readonly int limitMax = 999;

        public Player Player { get; private set; }
        public Battle Battle { get; private set; }

        public static MusicPlayer bgmPlayer, sePlayer;

        public Action? SetLabelBaseCol;
        public Action? ChangeLblText;
        public Action? KeyUpPressed;
        public Action? KeyDownPressed;
        public Action? KeyLeftPressed;
        public Action? KeyRightPressed;
        public Action? SetMapPos;

        public Action? KeyIPressed;
        public Action? KeyPPressed;
        public Action? KeyOPressed;
        public Action<FadeForm.FadeDir>? StartFade;

        public Func<int, int, int, bool, Task>? CallDrop;
        public Action<Image>? SetMonsterImg;

        private Point eventPos = Point.Empty;
        private Point prePos = Point.Empty;

        public GameManager()
        {
            Map.ReadFromCsv(Const.mapCsv);//最初に実行する事!

            EventData.ReadFromCsv(Const.eventCsv);
            MonsterData.ReadFromCsv(Const.monsterCsv);
            ItemData.ReadFromCsv(Const.itemCsv);

            Player = new Player(_playerName, _playerHp, _playerAttack, limitMax);
            Battle = new Battle(Player);

            bgmPlayer = new MusicPlayer();
            sePlayer = new MusicPlayer();

            DrawMessage.Setup();
        }

        public void KeyInput(Keys keyCode)
        {
            if (gameMode == GameMode.Explore)
            {
                if (keyCode == Keys.Up || keyCode == Keys.Down || keyCode == Keys.Left || keyCode == Keys.Right)
                {
                    // メッセージ表示中はメッセージ処理優先
                    if (DrawMessage.isMessageShowing)
                    {
                        DrawMessage.InputKey();
                        return;
                    }

                    Move(keyCode);
                }
                else if (keyCode == Keys.P)
                {
                    KeyPPressed?.Invoke();
                }
                else if (keyCode == Keys.O)
                {
                    KeyOPressed?.Invoke();
                }
                else if (keyCode == Keys.I)
                {
                    KeyIPressed?.Invoke();
                }
                //else if (keyCode == Keys.V)//デバッグ用
                //{
                //    IsVisionEnabled = !IsVisionEnabled;
                //}
            }
            else if (gameMode == GameMode.Battle)
            {
                if (keyCode == Keys.Up)
                {
                    KeyUpPressed?.Invoke();
                }
                else if (keyCode == Keys.Down)
                {
                    KeyDownPressed?.Invoke();
                }
                else if (keyCode == Keys.Left)
                {
                    KeyLeftPressed?.Invoke();
                }
                else if (keyCode == Keys.Right)
                {
                    KeyRightPressed?.Invoke();
                }
            }
        }

        public void Init()
        {
            Player.Init(_playerHp, limitMax);
            eventPos = Point.Empty;
            IsVisionEnabled = true;
        }

        public async Task BattleCheckAsync()
        {
            if (gameMode == GameMode.Escaped)
            {
                Form1.isBattleInputLocked = true;
                Form1.battleInputUnlockTime = DateTime.Now.AddSeconds(2.5); // 指定秒間キー入力をロック
                Map.playerPos = prePos;
                SetMapPos?.Invoke();
                await Task.Delay(500);
                gameMode = GameMode.Explore;
                ChangeLblText?.Invoke();
            }

            if (gameMode == GameMode.BattleEnd)
            {
                if (Player.Hp > 0)
                {
                    await DrawMessage.ShowAsync($"{Player.Name}は勝利した!");
                    await Task.Delay(500);
                    // モンスターを倒したらイベントを消去
                    Map.DeleteEvent(eventPos.X, eventPos.Y);
                    //マップ上の敵シンボルを消す
                    Map.DelEnemySimbolDraw(eventPos.X, eventPos.Y);
                    eventPos = Point.Empty;
                    gameMode = GameMode.Explore;
                    ChangeLblText?.Invoke();
                    bgmPlayer.PlayLoop(Resources.maou_bgm_8bit04);
                }
            }

            if (gameMode == GameMode.Gameover) Gameover();
        }//BattleCheck

        private async void Move(Keys keyCode)
        {
            Point dir = Point.Empty;

            if (keyCode == Keys.Up)
            {
                dir = new Point(0, -1);
                Player.Dir = Player.Direction.Up;
            }
            else if (keyCode == Keys.Down)
            {
                dir = new Point(0, 1);
                Player.Dir = Player.Direction.Down;
            }
            else if (keyCode == Keys.Left)
            {
                dir = new Point(-1, 0);
                Player.Dir = Player.Direction.Left;
            }
            else if (keyCode == Keys.Right)
            {
                dir = new Point(1, 0);
                Player.Dir = Player.Direction.Right;
            }

            prePos = Map.playerPos; // 移動前の位置を保存

            Player.SetDirectionImage(Player.Dir);
            Point newPos = new Point(Map.playerPos.X + dir.X, Map.playerPos.Y + dir.Y);

            // 移動可能かチェック
            if (Map.CanMoveTo(newPos.X, newPos.Y))
            {
                eventPos = newPos; // モンスターイベント位置を保存

                Map.playerPos = newPos;
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
                Player.TakeDamage(DamageFloorValue);
                isDamaged = true;
            }
            if (Player.Status == Status.Poison)
            {
                Player.TakeDamage(PoisonDamageValue);
                isDamaged = true;
            }

            if(isDamaged)
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
                gameMode = GameMode.Gameover; Gameover();
            }

            var invertCount = limitMax - Player.Limit;
            if (invertCount % ViewShrinkInterval == 0) Map.AddViewRadius(-1);
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
                    gameMode = GameMode.GameClear;
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
            GameManager.bgmPlayer.PlayLoop(Properties.Resources.maou_bgm_8bit08);

            Form1.isBattleInputLocked = true;
            Form1.battleInputUnlockTime = DateTime.Now.AddSeconds(3.0); // 指定秒間キー入力をロック

            gameMode = GameMode.Battle;
            var mon = MonsterData.Dict[evt.Word];
            Battle.Monster = new Monster(mon.Name, mon.Hp, mon.Attack, mon.Weak, mon.ImageName, mon.behavior);

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
            playerImage.Visible = Map.WalkMap[Map.playerPos.X, Map.playerPos.Y] != 2 ? true : false;
        }

    }//class
}//namespace
