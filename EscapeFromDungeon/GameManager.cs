using System;
using System.Drawing.Imaging;
using System.Reflection;
using System.Windows.Forms;

namespace EscapeFromDungeon
{
    public enum GameMode
    {
        Title,
        Explore,
        Battle,
        Escaped,
        BattleEnd,
        Gameover
    }

    internal class GameManager
    {
        public static GameMode gameMode { get; set; } = GameMode.Explore;
        // true: 視界制限あり、false: 全体表示
        public static bool IsVisionEnabled { get; set; } = true;

        private const string mapCsv = "map.csv";
        private const string eventCsv = "event.csv";
        private const string monsterCsv = "monster.csv";
        private const string itemCsv = "item.csv";
        private const string playerName = "あなた";
        private const int ViewShrinkInterval = 33;
        private const int DamageFloorValue = 3;
        private const int PoisonDamageValue = 1;
        private const int _limitMax = 999;

        public Player Player { get; private set; }
        public Map Map { get; private set; }
        public Message Message { get; private set; }
        public Battle Battle { get; private set; }

        public Action ChangeLblText;
        public Action KeyUpPressed;
        public Action KeyDownPressed;
        public Action KeyLeftPressed;
        public Action KeyRightPressed;

        private EventData _eventData;
        private MonsterData _monsterData;
        private ItemData _itemData;

        public GameManager()
        {
            Map = new Map(mapCsv);//最初に生成する事!

            _eventData = new EventData(eventCsv);
            _monsterData = new MonsterData(monsterCsv);
            _itemData = new ItemData(itemCsv);

            Player = new Player(playerName, 100, 10);
            Player.Limit = _limitMax;
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
                SwitchView(keyCode, overlayBox);
            }
            else if (gameMode == GameMode.Battle)
            {
                if (keyCode == Keys.Up)
                {
                    KeyUpPressed.Invoke();
                }
                else if (keyCode == Keys.Down)
                {
                    KeyDownPressed.Invoke();
                }
                else if (keyCode == Keys.Left)
                {
                    KeyLeftPressed.Invoke();
                }
                else if (keyCode == Keys.Right)
                {
                    KeyRightPressed.Invoke();
                }
            }
        }

        public async Task BattleCheckAsync()
        {
            if (gameMode == GameMode.Escaped)
            {
                Message.Show($"{Player.Name}は逃げ出した！");
                await Task.Delay(500);
                gameMode = GameMode.Explore;
                ChangeLblText.Invoke();
            }

            if (gameMode == GameMode.BattleEnd)
            {
                if (Player.Hp > 0)
                {
                    Message.Show($"{Player.Name}は勝利した!");
                    await Task.Delay(500);
                    DeleteEncountEvent();
                    gameMode = GameMode.Explore;
                    ChangeLblText.Invoke();
                }
            }

            if (gameMode == GameMode.Gameover) Gameover();
        }//BattleCheck

        // バトルイベントを消去(向いている方向の一歩前)
        private void DeleteEncountEvent()
        {
            if (Player.Dir == Player.Direction.Up)
                Map.EventMap[Map.playerPos.X, Map.playerPos.Y - 1] = null;
            else if (Player.Dir == Player.Direction.Down)
                Map.EventMap[Map.playerPos.X, Map.playerPos.Y + 1] = null;
            else if (Player.Dir == Player.Direction.Left)
                Map.EventMap[Map.playerPos.X - 1, Map.playerPos.Y] = null;
            else if (Player.Dir == Player.Direction.Right)
                Map.EventMap[Map.playerPos.X + 1, Map.playerPos.Y] = null;
        }

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
                    // 戦闘イベントなら移動せずに終了
                    return;
                }

                // 移動後処理
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
                Player.Hp -= DamageFloorValue;
            }
            if (Player.Status == Status.Poison)
            {
                Player.Hp -= PoisonDamageValue;
            }
        }

        //デバッグ用
        private void SwitchView(Keys keyCode, PictureBox overlayBox)
        {
            if (keyCode == Keys.V)
            {
                IsVisionEnabled = !IsVisionEnabled;
                if (IsVisionEnabled)
                    Map.DrawBrightness(overlayBox);
                else
                {
                    Map.ClearBrightness(overlayBox);
                }
            }
        }

        private void TurnCheck()
        {
            Player.Limit--;
            if (Player.Limit <= 0 || Player.Hp <= 0) Gameover();
            var invertCount = _limitMax - Player.Limit;
            if (invertCount % ViewShrinkInterval == 0) Map.viewRadius--;
        }

        private Event CheckEvent(int x, int y)
        {
            var eventId = Map.EventMap[x, y];

            if (eventId == null) return null; // イベントなし

            Event evt = _eventData.Dict[eventId];

            if (!_eventData.Dict.ContainsKey(eventId)) return null;

            if (evt != null)
            {
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
                    default:
                        break;
                }

                if (evt.EventType == EventType.Hint || evt.EventType == EventType.Encount)
                {
                    // ヒント以外は一度きり,バトルは逃げた場合残すのでここでは消去しない
                }
                else
                {
                    Map.EventMap[x, y] = null; // イベントを消去
                }

                return evt;
            }

            return null;
        }//CheckEvent   

        private async void EncounterEventAsync(Event evt)
        {
            Form1.isBattleInputLocked = true;
            Form1.battleInputUnlockTime = DateTime.Now.AddSeconds(1.5); // 1.5秒間ロック

            gameMode = GameMode.Battle;
            var mon = _monsterData.Dict[evt.Word];
            Battle.Monster = new Monster(mon.Name, mon.Hp, mon.Attack, mon.Weak);
            Battle.BattleTurn = 0;

            Battle.SetMonsterVisible.Invoke(true);
            ChangeLblText.Invoke();

            await Message.ShowAsync($"{mon.Name}が現れた！");
            await Task.Delay(500);

            await Message.ShowAsync($"コマンド？");
            Battle.SetButtonEnabled.Invoke(true);
            await Task.Delay(100);
        }

        private void ItemGetEvent(Event evt)
        {
            var item = _itemData.Dict[evt.Word];
            var name = item.Name;
            var dsc = item.Description;
            Player.Inventry.Add(new Item(name, dsc));
            Message.Show($"アイテム「{evt.Word}」を取得！");
        }

        private void HealEvent(Event evt)
        {
            Message.Show(evt.Word);

            switch (evt.Id)
            {
                case "L0":
                    Player.Hp += 20;
                    break;
                case "L1":
                    Player.Hp += 50;
                    break;
                case "L2":
                    Player.Hp += 100;
                    break;
                case "L3":
                    Player.Status = Status.Normal;
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
                    Player.Hp -= 1;
                    break;
                case "T1":
                    Player.Hp -= 20;
                    break;
                case "T3":
                    Player.Status = Status.Poison;
                    break;
                default:
                    break;
            }
        }

        private void Gameover()
        {
            MessageBox.Show("Game Over!");
            Application.Exit();
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
