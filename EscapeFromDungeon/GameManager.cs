using System;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace EscapeFromDungeon
{
    public enum GameMode
    {
        Title,
        Explore,
        Battle,
        Gameover
    }

    internal class GameManager
    {
        private const string mapCsv = "map.csv";
        private const string eventCsv = "event.csv";
        private const string monsterCsv = "monster.csv";
        private const string itemCsv = "item.csv";
        private const string playerName = "あなた";

        public Player player { get; private set; }

        private int turnCount = 0;

        public Map Map { get; private set; }
        private EventData eventData;
        private MonsterData monsterData;
        private ItemData itemData;
        public Message Message { get; private set; }

        // true: 視界制限あり、false: 全体表示
        public static bool IsVisionEnabled { get; set; } = true;

        private bool isMoving = false;
        private System.Windows.Forms.Timer waitTimer;

        public GameManager()
        {
            Map = new Map(mapCsv);
            player = new Player(playerName, 100, 10);
            eventData = new EventData(eventCsv);
            monsterData = new MonsterData(monsterCsv);
            itemData = new ItemData(itemCsv);
            Message = new Message();
            waitTimer = new System.Windows.Forms.Timer();
            waitTimer.Tick += UiTimer_Tick;
        }

        private void UiTimer_Tick(object? sender, EventArgs e)
        {
            isMoving = false;
            waitTimer.Stop();
        }

        public void KeyInput(Keys keyCode, PictureBox mapImage, PictureBox overlayBox)
        {
            // メッセージ中は移動操作を無効化
            if (Message.isMessageShowing)
            {
                // メッセージ表示中はスペースで全文表示 or 次のメッセージ
                if (keyCode == Keys.Space) Message.InputKey();
                return;
            }

            if (isMoving) return; // 移動中は入力を無視

            SwitchView(keyCode, overlayBox);
            Move(keyCode, mapImage, overlayBox);
            WaitTimerStart();
        }

        private void Move(Keys keyCode, PictureBox mapImage, PictureBox overlayBox)
        {
            Point dir = Point.Empty;
            int moveAmount = 32; // 移動量（ピクセル）

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

            player.GetPlayerImage(player.Dir);
            Point newPos = new Point(Map.playerPos.X + dir.X, Map.playerPos.Y + dir.Y);

            // 移動可能かチェック
            if (Map.CanMoveTo(newPos.X, newPos.Y))
            {
                Map.playerPos = newPos;
                mapImage.Location = current1;
                overlayBox.Location = current2;
                CheckEvent(newPos.X, newPos.Y);
                DamageCheck(newPos.X, newPos.Y);
                TurnCheck();
            }
        }

        private void DamageCheck(int x, int y)
        {
            if (Map.BaseMap[x, y] == 3)
            {
                player.Hp -= 3;
            }
            if (player.Status == Status.Poison)
            {
                player.Hp -= 1;
            }
        }

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

        private void WaitTimerStart()
        {
            isMoving = true;
            waitTimer.Interval = 56;
            waitTimer.Start();
        }

        private void TurnCheck()
        {
            turnCount++;
            player.Limit--;
            if (player.Limit <= 0 || player.Hp <= 0) Gameover();
            if (turnCount % 33 == 0) { Map.viewRadius--; }
        }

        private void CheckEvent(int x, int y)
        {
            var cell = Map.EventMap[x, y];

            Event evt = eventData.eventDatas.Find(e => e.Id == cell);
            if (evt != null)
            {
                switch (evt.EventType)
                {
                    case EventType.Message:
                        Message.Show(evt.Word);
                        break;
                    case EventType.ItemGet:
                        ApplyItemGetEvent(evt);
                        break;
                    case EventType.Heal:
                        ApplyHealEffect(evt);
                        break;
                    case EventType.Trap:
                        ApplyTrapEffect(evt);
                        break;
                    case EventType.EnemyEncount:
                        ApplyEnemyEncounterEvent(evt);
                        break;
                    default:
                        break;
                }

                // メッセージイベント以外は一度きり
                if (evt.EventType != EventType.Message)
                {
                    Map.EventMap[x, y] = "00"; // イベントを消去
                }
            }
        }

        private void ApplyEnemyEncounterEvent(Event evt)
        {
            Message.Show($"{evt.Word} が現れた！");
            var monster = monsterData.monsterDatas.Find(x => x.Name == evt.Word);
            var battle = new Battle(monster, player, Message);
            var winner = battle.BattleLoop();
            if (winner.Name == playerName)
            {
                Message.Show($"{playerName}は勝利した！");
            }
        }

        private void ApplyItemGetEvent(Event evt)
        {
            var item = itemData.ItemDatas.Find(x => x.Name == evt.Word);
            var name = item.Name;
            var dsc = item.Description;
            player.Inventry.Add(new Item(name, dsc));
            Message.Show($"アイテム「{evt.Word}」を取得！");
        }

        private void ApplyHealEffect(Event evt)
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

        private void ApplyTrapEffect(Event evt)
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
            playerImage.Visible = Map.BaseMap[Map.playerPos.X, Map.playerPos.Y] != 2 ? true : false;
        }

    }//class
}//namespace
