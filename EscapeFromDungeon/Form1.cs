
using EscapeFromDungeon.Properties;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    public partial class Form1 : Form
    {
        private readonly Color btnBaseCol = Color.DarkGray;
        private readonly Color btnSelectCol = Color.DarkOrange;

        private PictureBox mapImage = default!, overlayImg = default!, playerImg = default!, monsterImg = default!;
        private GameManager gameManager;
        private DrawInfo drawInfo;

        private Dictionary<Label, string> itemMap;

        private System.Windows.Forms.Timer timer = default!;//画面表示更新用
        private const int timerInterval = 32;

        public static bool isBattleInputLocked = false;
        public static DateTime battleInputUnlockTime;

        private DateTime lastInputTime = DateTime.MinValue;
        private readonly TimeSpan inputCooldown = TimeSpan.FromMilliseconds(500);

        private bool _isWaiting = false;

        public Form1()
        {
            InitializeComponent();
            gameManager = new GameManager();//最初に生成する事!
            drawInfo = new DrawInfo();

            InitPictureBoxes();

            itemMap = new Dictionary<Label, string>
            {
                { lblUsePosion, Const.potion },
                { lblUseCurePoison, Const.curePoison },
                { lblUseTorch, Const.torch }
            };

            MsgBox.Location = new Point(10, 440);

            gameManager.CallDrop = DropEnemyAsync;
            gameManager.Battle.CallDrop = DropEnemyAsync;
            gameManager.Battle.CallShaker = ShakeAsync;
            gameManager.Battle.CallShrink = ShrinkEnemyAsync;
            gameManager.Battle.SetButtonEnabled = SetBattleButtonsEnabled;
            gameManager.Battle.SetMonsterVisible = SetMonsterVisible;
            gameManager.Battle.ChangeLblText = ChangeLblText;
            gameManager.ChangeLblText = ChangeLblText;
            gameManager.KeyUpPressed = () => lblAttackClickAsynk(this, EventArgs.Empty);
            gameManager.KeyLeftPressed = () => lblDefenceClickAsync(this, EventArgs.Empty);
            gameManager.KeyRightPressed = () => lblHealClickAsync(this, EventArgs.Empty);
            gameManager.KeyDownPressed = () => lblEscapeClickAsync(this, EventArgs.Empty);

            ChangeLblText();

            gameManager.Map.Draw(mapImage);
            gameManager.Map.DrawBrightness(overlayImg);
            gameManager.SetMapPos(mapImage, overlayImg, playerImg);

            DispPoint();

            timer = new System.Windows.Forms.Timer();
            timer.Interval = timerInterval;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        // 画面更新タイマーイベント
        private void Timer_Tick(object? sender, EventArgs e)
        {
            gameManager.Map.ClearBrightness(overlayImg);
            gameManager.Map.DrawBrightness(overlayImg);
            playerImg.Image = gameManager.Player.playerImage;
            gameManager.PlayerVisible(playerImg);
            overlayImg.Invalidate();
            StateBox.Invalidate();
            MsgBox.Invalidate();
            VisiblelblUse();
            DispPoint();
        }

        public void ChangeLblText()
        {
            if (GameManager.gameMode == GameMode.Battle)
            {
                lblAttack.Text = "攻撃";
                lblDefence.Text = "防御";
                lblHeal.Text = "回復";
                lblEscape.Text = "逃げる";
            }
            else
            {
                lblAttack.Text = "[↑]";
                lblDefence.Text = "[←]";
                lblHeal.Text = "[→]";
                lblEscape.Text = "[↓]";
            }
        }

        private void InitPictureBoxes()
        {
            //マップ画像の枠　入れ子の親
            mapDrawBox.Size = new Size(Map.tileSize * 13, Map.tileSize * 13);

            // マップ画像　入れ子の子
            mapImage = new PictureBox
            {
                Size = new Size(gameManager.Map.Width * Map.tileSize, gameManager.Map.Height * Map.tileSize),
                Location = Point.Empty,
                Image = gameManager.Map.MapCanvas,
                SizeMode = PictureBoxSizeMode.Normal
            };

            mapDrawBox.Controls.Add(mapImage);

            // オーバーレイ画像
            overlayImg = new PictureBox
            {
                Size = mapDrawBox.Size,
                Location = Point.Empty,
                BackColor = Color.Transparent,
                Image = gameManager.Map.overrayCanvas,
                Parent = mapImage // mapImageの上に重ねる
            };

            overlayImg.BringToFront(); // mapImageの上に表示

            // プレイヤー画像
            playerImg = new PictureBox
            {
                Size = new(Map.tileSize, Map.tileSize),
                Location = new Point(Map.playerPos.X * Map.tileSize, Map.playerPos.Y * Map.tileSize),
                BackColor = Color.Transparent,
                Image = gameManager.Player.playerImage,
                Parent = overlayImg
            };

            overlayImg.Controls.Add(playerImg);
            playerImg.BringToFront(); // overlayの上に表示

            // モンスター画像
            monsterImg = new PictureBox
            {
                Size = new(Map.tileSize * 8, Map.tileSize * 8),
                Location = new Point(96, 96),
                BackColor = Color.Transparent,
                Image = Resources.Enemy01,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            this.Controls.Add(monsterImg);

            monsterImg.BringToFront();
        }//InitPictureBoxes

        private void MainFormKeyDown(object sender, KeyEventArgs e)
        {
            gameManager.KeyInput(e.KeyCode, mapImage, overlayImg);
        }

        private void StateBoxPaint(object sender, PaintEventArgs e)
        {
            drawInfo.DrawStatus(e.Graphics, gameManager.Player);
        }

        private void MsgBoxPaint(object sender, PaintEventArgs e)
        {
            gameManager.Message.Draw(e.Graphics);
        }

        private async void lblAttackClickAsynk(object sender, EventArgs e)
        {
            if (isBattleInputLocked && DateTime.Now < battleInputUnlockTime) return;
            if (_isWaiting) return;
            if (DateTime.Now - lastInputTime < inputCooldown) return;

            _isWaiting = true;
            lastInputTime = DateTime.Now;

            if (GameManager.gameMode == GameMode.Battle)
            {
                SetBattleButtonsEnabled(false);
                await gameManager.Battle.PlayerTurnAsync(Const.CommandAtk);
                await gameManager.BattleCheckAsync();
            }
            else if (GameManager.gameMode == GameMode.Explore)
            {
                gameManager.KeyInput(Keys.Up, mapImage, overlayImg);
            }

            _isWaiting = false;
        }
        private async void lblAttackClickAsync(object sender, EventArgs e)
            => await HandleBattleOrExploreAsync(Const.CommandAtk, Keys.Up);

        private async void lblDefenceClickAsync(object sender, EventArgs e)
            => await HandleBattleOrExploreAsync(Const.CommandDef, Keys.Left);

        private async void lblHealClickAsync(object sender, EventArgs e)
            => await HandleBattleOrExploreAsync(Const.CommandHeal, Keys.Right);

        private async void lblEscapeClickAsync(object sender, EventArgs e)
            => await HandleBattleOrExploreAsync(Const.CommandEsc, Keys.Down);

        private async Task HandleBattleOrExploreAsync(string command, Keys exploreKey)
        {
            if (isBattleInputLocked && DateTime.Now < battleInputUnlockTime) return;
            if (_isWaiting) return;
            if (DateTime.Now - lastInputTime < inputCooldown) return;

            _isWaiting = true;
            lastInputTime = DateTime.Now;

            if (GameManager.gameMode == GameMode.Battle)
            {
                SetBattleButtonsEnabled(false);
                await gameManager.Battle.PlayerTurnAsync(command);
                await gameManager.BattleCheckAsync();
            }
            else if (GameManager.gameMode == GameMode.Explore)
            {
                gameManager.KeyInput(exploreKey, mapImage, overlayImg);
            }

            _isWaiting = false;
        }

        public async Task ShakeAsync(int targetNum, int critical, int durationMs = 500, int intervalMs = 30)
        {
            PictureBox target = default!;
            if (targetNum == 1) target = MsgBox;
            else if (targetNum == 2) target = monsterImg;

            var originalLocation = target.Location;
            var rand = new Random();
            int shakeCount = durationMs / intervalMs;

            for (int i = 0; i < shakeCount; i++)
            {
                int offsetX, offsetY;
                if (critical == 2)
                {
                    // クリティカルヒット時は大きく揺らす
                    offsetX = rand.Next(-12, 13);
                    offsetY = rand.Next(-12, 13);
                }
                else
                {
                    // 通常の揺れ
                    offsetX = rand.Next(-6, 7);
                    offsetY = rand.Next(-6, 7);
                }

                target.Location = new Point(originalLocation.X + offsetX, originalLocation.Y + offsetY);

                await Task.Delay(intervalMs);
            }

            target.Location = originalLocation;
        }

        public async Task DropEnemyAsync(int dropDistance = 600, int step = 10, int intervalMs = 16, bool isIntro = false)
        {
            Point originalLocation = monsterImg.Location;

            if (isIntro)
            {
                // バトル開始時：上から落ちてくる
                int startY = originalLocation.Y - dropDistance;
                monsterImg.Location = new Point(originalLocation.X, startY);
                monsterImg.Visible = true;

                int steps = dropDistance / step;
                for (int i = 0; i < steps; i++)
                {
                    monsterImg.Location = new Point(originalLocation.X, monsterImg.Location.Y + step);
                    await Task.Delay(intervalMs);
                }

                monsterImg.Location = originalLocation;
            }
            else
            {
                // 敵撃破時：下に落ちて消える
                int steps = dropDistance / step;
                for (int i = 0; i < steps; i++)
                {
                    monsterImg.Location = new Point(originalLocation.X, monsterImg.Location.Y + step);
                    await Task.Delay(intervalMs);
                }

                monsterImg.Visible = false;
                monsterImg.Location = originalLocation;
            }
        }

        public async Task ShrinkEnemyAsync(int steps = 20, int intervalMs = 30)
        {
            var originalSize = monsterImg.Size;
            var originalLocation = monsterImg.Location;

            for (int i = 0; i < steps; i++)
            {
                // 割合を計算（徐々に小さく）
                float scale = 1f - (i + 1f) / steps;

                int newWidth = (int)(originalSize.Width * scale);
                int newHeight = (int)(originalSize.Height * scale);

                // 中心を保つように位置を調整
                int offsetX = (originalSize.Width - newWidth) / 2;
                int offsetY = (originalSize.Height - newHeight) / 2;

                monsterImg.Size = new Size(newWidth, newHeight);
                monsterImg.Location = new Point(originalLocation.X + offsetX, originalLocation.Y + offsetY);

                await Task.Delay(intervalMs);
            }

            // 完了後に非表示＆サイズ・位置を初期化
            monsterImg.Visible = false;
            monsterImg.Size = originalSize;
            monsterImg.Location = originalLocation;
        }

        private void SetBattleButtonsEnabled(bool enabled)
        {
            lblAttack.Visible = enabled;
            lblDefence.Visible = enabled;
            lblHeal.Visible = enabled;
            lblEscape.Visible = enabled;
        }

        public void LblHealVisible()
        {
            if (GameManager.gameMode != GameMode.Battle)
            {
                int potionCount = gameManager.Player.GetItemCount(Const.potion);
                lblHeal.Visible = potionCount > 0;
            }
        }

        private void SetMonsterVisible(bool visible) => monsterImg.Visible = visible;

        private void LabelMouseHover(object sender, EventArgs e)
        {
            if (sender is Label lbl) lbl.BackColor = btnSelectCol;
        }

        private void LabelMouseLeave(object sender, EventArgs e)
        {
            if (sender is Label lbl) lbl.BackColor = btnBaseCol;
        }

        private void VisiblelblUse()
        {
            int potionCount = gameManager.Player.GetItemCount(Const.potion);
            int curePoisonCount = gameManager.Player.GetItemCount(Const.curePoison);
            int torchCount = gameManager.Player.GetItemCount(Const.torch);
            lblUsePosion.Visible = GameManager.gameMode == GameMode.Explore && potionCount > 0;
            lblUseCurePoison.Visible = GameManager.gameMode == GameMode.Explore && curePoisonCount > 0;
            lblUseTorch.Visible = GameManager.gameMode == GameMode.Explore && torchCount > 0;
        }

        private void ItemLabelClick(object sender, EventArgs e)
        {
            if (sender is Label lbl && itemMap.TryGetValue(lbl, out string itemName))
            {
                UseItem(itemName);
            }
        }

        private void UseItem(string itemName)
        {
            switch (itemName)
            {
                case Const.potion:
                    if (gameManager.Player.Hp == gameManager.Player.MaxHp)
                    {
                        gameManager.Message.Show(Const.hpFullMsg);
                        return;
                    }
                    gameManager.Player.Heal(30);
                    break;

                case Const.curePoison:
                    gameManager.Player.HealStatus();
                    break;

                case Const.torch:
                    Map.viewRadius = 12;
                    break;
            }

            gameManager.Message.Show($"{itemName}を使った！");
            gameManager.Player.UseItem(itemName);
        }


        // デバッグ用
        private void DispPoint()
        {
            label1.Text = $"XY:({Map.playerPos.X}, {Map.playerPos.Y}) limit:{gameManager.Player.Limit} " +
                $"mi:({mapImage.Location.X},{mapImage.Location.Y}) " +
                $"oi:({overlayImg.Location.X},{overlayImg.Location.Y}) pi:({playerImg.Location.X},{playerImg.Location.Y})";
        }

    }//class
}//namespace
