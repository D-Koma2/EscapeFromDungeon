using EscapeFromDungeon.Properties;
using Microsoft.VisualBasic;
using System;
using System.Drawing.Imaging;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using WindowsFormsAppTest2;

namespace EscapeFromDungeon
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
        private const int _limitMax = 555;

        public Player Player { get; private set; }
        public Map Map { get; private set; }
        public Message Message { get; private set; }
        public Battle Battle { get; private set; }

        public Action? SetLabelBaseCol;
        public Action? ChangeLblText;
        public Action? KeyUpPressed;
        public Action? KeyDownPressed;
        public Action? KeyLeftPressed;
        public Action? KeyRightPressed;

        public Action? KeyIPressed;
        public Action? KeyPPressed;
        public Action? KeyOPressed;
        public Action<FadeForm.FadeDir>? StartFade;

        public Func<int, int, int, bool, Task>? CallDrop;
        public Action<Image>? SetMonsterImg;

        private EventData _eventData;
        private MonsterData _monsterData;
        private ItemData _itemData;

        private Point eventPos = Point.Empty;

        public GameManager()
        {
            Map = new Map(Const.mapCsv);//最初に生成する事!

            _eventData = new EventData(Const.eventCsv);
            _monsterData = new MonsterData(Const.monsterCsv);
            _itemData = new ItemData(Const.itemCsv);

            Player = new Player(_playerName, _playerHp, _playerAttack, _limitMax);
            Message = new Message();
            Battle = new Battle(Player, Message);
        }

        public void KeyInput(Keys keyCode, PictureBox mapImage, PictureBox overlayBox)
        {
            if (gameMode == GameMode.Explore)
            {
                if (keyCode == Keys.Up || keyCode == Keys.Down || keyCode == Keys.Left || keyCode == Keys.Right)
                {
                    // メッセージ表示中はメッセージ処理優先
                    if (Message.isMessageShowing)
                    {
                        Message.InputKey();
                        return;
                    }

                    Move(keyCode, mapImage, overlayBox);
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
                else if (keyCode == Keys.V)
                {
                    SwitchView(overlayBox);
                }
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
            Player.Init(_playerHp,_limitMax);
            eventPos = Point.Empty;
        }

        public async Task BattleCheckAsync()
        {
            if (gameMode == GameMode.Escaped)
            {
                await Task.Delay(500);
                gameMode = GameMode.Explore;
                ChangeLblText?.Invoke();
            }

            if (gameMode == GameMode.BattleEnd)
            {
                if (Player.Hp > 0)
                {
                    Message.Show($"{Player.Name}は勝利した!");
                    await Task.Delay(500);
                    // モンスターを倒したらイベントを消去
                    Map.DeleteEvent(eventPos.X, eventPos.Y);
                    //マップ上の敵シンボルを消す
                    Map.DelEnemySimbolDraw(eventPos.X, eventPos.Y);
                    eventPos = Point.Empty;
                    gameMode = GameMode.Explore;
                    ChangeLblText?.Invoke();
                }
            }

            if (gameMode == GameMode.Gameover) Gameover();
        }//BattleCheck

        private void Move(Keys keyCode, PictureBox mapImage, PictureBox overlayBox)
        {
            Point dir = Point.Empty;
            int moveAmount = Map.tileSize; // 移動量（ピクセル）

            Point current1 = mapImage.Location;
            Point current2 = overlayBox.Location;

            if (keyCode == Keys.Up)
            {
                current1.Y += moveAmount;
                current2.Y -= moveAmount;
                dir = new Point(0, -1);
                Player.Dir = Player.Direction.Up;
            }
            else if (keyCode == Keys.Down)
            {
                current1.Y -= moveAmount;
                current2.Y += moveAmount;
                dir = new Point(0, 1);
                Player.Dir = Player.Direction.Down;
            }
            else if (keyCode == Keys.Left)
            {
                current1.X += moveAmount;
                current2.X -= moveAmount;
                dir = new Point(-1, 0);
                Player.Dir = Player.Direction.Left;
            }
            else if (keyCode == Keys.Right)
            {
                current1.X -= moveAmount;
                current2.X += moveAmount;
                dir = new Point(1, 0);
                Player.Dir = Player.Direction.Right;
            }

            Player.SetDirectionImage(Player.Dir);
            Point newPos = new Point(Map.playerPos.X + dir.X, Map.playerPos.Y + dir.Y);

            // 移動可能かチェック
            if (Map.CanMoveTo(newPos.X, newPos.Y))
            {
                Event evt = CheckEvent(newPos.X, newPos.Y);

                if (evt == null) Message.Init(); // メッセージリセット

                if (evt != null && evt.EventType == EventType.Encount)
                {
                    eventPos = newPos; // モンスターイベント位置を保存
                    return;
                }

                Map.playerPos = newPos;
                mapImage.Location = current1;
                overlayBox.Location = current2;
                DamageCheck(newPos.X, newPos.Y);
                TurnCheck();
            }
        }//Move 

        private void DamageCheck(int x, int y)
        {
            // ダメージ床
            if (Map.WalkMap[x, y] == 3)
            {
                Player.TakeDamage(DamageFloorValue);
            }
            if (Player.Status == Status.Poison)
            {
                Player.TakeDamage(PoisonDamageValue);
            }
        }

        //デバッグ用
        private void SwitchView(PictureBox overlayBox)
        {
            IsVisionEnabled = !IsVisionEnabled;
            if (IsVisionEnabled)
                Map.DrawBrightness(overlayBox);
            else
            {
                Map.ClearBrightness(overlayBox);
            }
        }

        private void TurnCheck()
        {
            Player.Limit--;

            if (Player.Limit <= 0 || Player.Hp <= 0)
            {
                gameMode = GameMode.Gameover; Gameover();
            }

            var invertCount = _limitMax - Player.Limit;
            if (invertCount % ViewShrinkInterval == 0) Map.AddViewRadius(-1);
        }

        private Event CheckEvent(int x, int y)
        {
            var eventId = Map.EventMap[x, y];

            if (eventId == null) return null; // イベントなし

            Event evt = _eventData.Dict[eventId];

            if (!_eventData.Dict.ContainsKey(eventId)) return null;

            switch (evt.EventType)
            {
                case EventType.Message:
                    Message.Show(evt.Word);
                    break;
                case EventType.Hint:
                    Message.Show(evt.Word);
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
                    gameMode = GameMode.GameClear;
                    Gameover();
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
            Form1.isBattleInputLocked = true;
            Form1.battleInputUnlockTime = DateTime.Now.AddSeconds(1.5); // 1.5秒間ロック

            gameMode = GameMode.Battle;
            var mon = _monsterData.Dict[evt.Word];
            Battle.Monster = new Monster(mon.Name, mon.Hp, mon.Attack, mon.Weak, mon.ImageName);

            //モンスターイメージを変更
            Image? img = Resources.ResourceManager.GetObject(mon.ImageName) as Image;
            if (img != null) SetMonsterImg?.Invoke(img);

            Battle.InitBattleTurn();

            if (CallDrop != null) await CallDrop.Invoke(600, 10, 4, true);
            ChangeLblText?.Invoke();

            await Message.ShowAsync($"{mon.Name}が現れた！");
            await Message.ShowAsync(Const.commndMsg);
            Battle.SetButtonEnabled?.Invoke(true);
            await Task.Delay(200);
        }

        private void ItemGetEvent(Event evt)
        {
            var item = _itemData.Dict[evt.Word];
            var name = item.Name;
            var dsc = item.Description;
            Player.GetItem(name, dsc);
            Message.Show($"アイテム「{evt.Word}」を取得！");
        }

        private void HealEvent(Event evt)
        {
            Message.Show(evt.Word);

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

        private void TrapEvent(Event evt)
        {
            Message.Show(evt.Word);

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
        }
        private void Gameover()
        {
            SetLabelBaseCol?.Invoke();
            StartFade(FadeForm.FadeDir.FadeIn);
        }

        // マップとプレイヤーの位置調整
        public void SetMapPos(PictureBox mapImage, PictureBox overlayBox, PictureBox playerImage)
        {
            int moveX = Map.tileSize * (Map.playerPos.X - 6);
            int moveY = Map.tileSize * (Map.playerPos.Y - 6);

            mapImage.Location = new Point(-moveX, -moveY);
            overlayBox.Location = new Point(moveX, moveY);
            playerImage.Location = new Point(Map.tileSize * 6, Map.tileSize * 6);
        }

        public void PlayerVisible(PictureBox playerImage)
        {
            playerImage.Visible = Map.WalkMap[Map.playerPos.X, Map.playerPos.Y] != 2 ? true : false;
        }

    }//class
}//namespace
