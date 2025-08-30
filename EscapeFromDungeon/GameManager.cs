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
        private const string mapCsv = "map.csv";
        private const string eventCsv = "event.csv";
        private const string monsterCsv = "monster.csv";
        private const string itemCsv = "item.csv";
        private const string playerName = "あなた";
        private const int ViewShrinkInterval = 33;
        private const int DamageFloorValue = 3;
        private const int PoisonDamageValue = 1;

        public static GameMode gameMode { get; set; } = GameMode.Explore;

        public Player player { get; private set; }

        private int turnCount = 0;

        public Battle Battle { get; private set; }
        public Action ChangeLblText;
        public Action KeyUpPressed;
        public Action KeyDownPressed;
        public Action KeyLeftPressed;
        public Action KeyRightPressed;

        public Map Map { get; private set; }

        private EventData eventData;
        private MonsterData monsterData;
        private ItemData itemData;
        public Message Message { get; private set; }

        // true: 視界制限あり、false: 全体表示
        public static bool IsVisionEnabled { get; set; } = true;

        public GameManager()
        {
            Map = new Map(mapCsv);//最初に生成する事!

            eventData = new EventData(eventCsv);
            monsterData = new MonsterData(monsterCsv);
            itemData = new ItemData(itemCsv);

            player = new Player(playerName, 100, 10);
            Message = new Message();
            Battle = new Battle(player, Message);
        }

        public void KeyInput(Keys keyCode, PictureBox mapImage, PictureBox overlayBox)
        {
            if (gameMode == GameMode.Explore)
            {
                // メッセージ表示中はメッセージ処理優先
                if (Message.isMessageShowing)
                {
                    Message.InputKey();
                    return;
                }

                SwitchView(keyCode, overlayBox);
                Move(keyCode, mapImage, overlayBox);
            }

            if (gameMode == GameMode.Battle)
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

        public async Task BattleCheck()
        {
            if (gameMode == GameMode.Escaped)
            {
                Message.Show($"{player.Name}は逃げ出した！");
                gameMode = GameMode.Explore;
                ChangeLblText.Invoke();
            }

            if (gameMode == GameMode.BattleEnd)
            {
                if (player.Hp > 0)
                {
                    Message.Show($"{player.Name}は勝利した!");
                    DeleteEncountEvent();
                    gameMode = GameMode.Explore;
                    ChangeLblText.Invoke();
                }
            }

            if (gameMode == GameMode.Gameover)
            {
                Gameover();
            }
        }//BattleCheck

        // バトルイベントを消去
        private void DeleteEncountEvent()
        {
            if (player.Dir == Player.Direction.Up)
                Map.EventMap[Map.playerPos.X, Map.playerPos.Y - 1] = null;
            else if (player.Dir == Player.Direction.Down)
                Map.EventMap[Map.playerPos.X, Map.playerPos.Y + 1] = null;
            else if (player.Dir == Player.Direction.Left)
                Map.EventMap[Map.playerPos.X - 1, Map.playerPos.Y] = null;
            else if (player.Dir == Player.Direction.Right)
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
                player.Dir = Player.Direction.Up;
            }
            else if (keyCode == Keys.Down)
            {
                current1.Y -= moveAmount;
                current2.Y += moveAmount;
                dir = new Point(0, 1);
                player.Dir = Player.Direction.Down;
            }
            else if (keyCode == Keys.Left)
            {
                current1.X += moveAmount;
                current2.X -= moveAmount;
                dir = new Point(-1, 0);
                player.Dir = Player.Direction.Left;
            }
            else if (keyCode == Keys.Right)
            {
                current1.X -= moveAmount;
                current2.X += moveAmount;
                dir = new Point(1, 0);
                player.Dir = Player.Direction.Right;
            }

            player.SetDirectionImage(player.Dir);
            Point newPos = new Point(Map.playerPos.X + dir.X, Map.playerPos.Y + dir.Y);

            // 移動可能かチェック
            if (Map.CanMoveTo(newPos.X, newPos.Y))
            {
                Event evt = CheckEvent(newPos.X, newPos.Y);

                if (evt == null) Message.Reset(); // メッセージリセット

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
                player.Hp -= DamageFloorValue;
            }
            if (player.Status == Status.Poison)
            {
                player.Hp -= PoisonDamageValue;
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
            turnCount++;
            player.Limit--;
            if (player.Limit <= 0 || player.Hp <= 0) Gameover();
            if (turnCount % ViewShrinkInterval == 0) { Map.viewRadius--; }
        }

        private Event CheckEvent(int x, int y)
        {
            var eventId = Map.EventMap[x, y];

            if (eventId == null) return null; // イベントなし

            Event evt = eventData.Dict[eventId];

            if (!eventData.Dict.ContainsKey(eventId)) return null;

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
                        EncounterEvent(evt);
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

        private async void EncounterEvent(Event evt)
        {
            Form1.isBattleInputLocked = true;
            Form1.battleInputUnlockTime = DateTime.Now.AddSeconds(1.5); // 1.5秒間ロック
            gameMode = GameMode.Battle;
            var mon = monsterData.Dict[evt.Word];
            Battle.Monster = new Monster(mon.Name, mon.Hp, mon.Attack, mon.Weak);
            Battle.TurnCount = 0;
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
            var item = itemData.Dict[evt.Word];
            var name = item.Name;
            var dsc = item.Description;
            player.Inventry.Add(new Item(name, dsc));
            Message.Show($"アイテム「{evt.Word}」を取得！");
        }

        private void HealEvent(Event evt)
        {
            Message.Show(evt.Word);

            switch (evt.Id)
            {
                case "L0":
                    player.Hp += 20;
                    break;
                case "L1":
                    player.Hp += 50;
                    break;
                case "L2":
                    player.Hp += 100;
                    break;
                case "L3":
                    player.Status = Status.Normal;
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
                    player.Hp -= 1;
                    break;
                case "T1":
                    player.Hp -= 20;
                    break;
                case "T3":
                    player.Status = Status.Poison;
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
