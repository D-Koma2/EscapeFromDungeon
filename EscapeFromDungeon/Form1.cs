
using EscapeFromDungeon.Properties;
using System.Diagnostics;
using System.Drawing;
using System.Security.Cryptography;
using System.Threading.Tasks;
using WindowsFormsAppTest2;

namespace EscapeFromDungeon
{
    public partial class Form1 : Form
    {
        private readonly Color lblBaseCol = Color.DarkGray;
        private readonly Color lblSelectCol = Color.DarkOrange;

        private PictureBox mapImage = default!, overlayImg = default!, playerImg = default!, monsterImg = default!;
        private FadeForm fade = default!;
        private GameManager gameManager;
        private DrawInfo drawInfo;

        private System.Windows.Forms.Timer timer = default!;//画面表示更新用
        private const int timerInterval = 32;
        private const int targetIsPlayer = 1;
        private const int targetIsEnemy = 2;
        private Dictionary<Label, string> itemMap;

        public static bool isBattleInputLocked = false;
        public static DateTime battleInputUnlockTime;

        private DateTime lastInputTime = DateTime.MinValue;
        private readonly TimeSpan inputCooldown = TimeSpan.FromMilliseconds(300);

        private bool _isWaiting = false;

        public Form1()
        {
            InitializeComponent();

            itemMap = new Dictionary<Label, string>
            {
                { lblUsePosion, Const.potion },
                { lblUseCurePoison, Const.curePoison },
                { lblUseTorch, Const.torch }
            };

            gameManager = new GameManager();//最初に生成する事!
            SetupGameManagerEvents();//GameManager生成の後に呼ぶ
            InitPictureBoxes();
            drawInfo = new DrawInfo();

            InitDraw();
            TimerSetUp();
            FadeSetup();

            //DispPoint();
        }

        public void InitGame()
        {
            GameManager.gameMode = GameMode.Title;
            gameManager.Map.InitMap(Const.mapCsv);
            gameManager.Init();
            gameManager.Message.Init();
            Map.AddViewRadius(Map.maxViewRadius);
            SetLabelBaseCol();
            InitDraw();

            //DispPoint();
        }

        private void InitDraw()
        {
            ChangeLblText();
            gameManager.Map.Draw(mapImage);
            gameManager.Map.DrawBrightness(overlayImg);
            gameManager.SetMapPos(mapImage, overlayImg, playerImg);
        }

        private void SetupGameManagerEvents()
        {
            gameManager.KeyUpPressed = () => lblAttackClickAsync(this, EventArgs.Empty);
            gameManager.KeyLeftPressed = () => lblDefenceClickAsync(this, EventArgs.Empty);
            gameManager.KeyRightPressed = () => lblHealClickAsync(this, EventArgs.Empty);
            gameManager.KeyDownPressed = () => lblEscapeClickAsync(this, EventArgs.Empty);

            gameManager.KeyPPressed = () => UseItem(Const.potion);
            gameManager.KeyOPressed = () => UseItem(Const.curePoison);
            gameManager.KeyIPressed = () => UseItem(Const.torch);

            gameManager.ChangeLblText = ChangeLblText;
            gameManager.SetMonsterImg = SetMonsterImage;
            gameManager.SetLabelBaseCol = SetLabelBaseCol;

            gameManager.CallDrop = DropEnemyAsync;
            gameManager.Battle.CallDrop = DropEnemyAsync;
            gameManager.Battle.CallShaker = ShakeAsync;
            gameManager.Battle.CallShrink = ShrinkEnemyAsync;

            gameManager.Battle.SetButtonEnabled = SetLabelVisible;
            gameManager.Battle.SetMonsterVisible = SetMonsterImgVisible;
            gameManager.Battle.ChangeLblText = ChangeLblText;
        }

        private void FadeSetup()
        {
            fade = new FadeForm(this, FadeForm.FadeDir.FadeOut); // MainForm を渡す
            gameManager.StartFade = fade.StartFade;

            // MainForm が移動したら FadeForm も追従
            this.LocationChanged += (_, __) => fade.FollowOwner();
            fade.InitStart = () => InitGame();
            fade.Show();
        }

        private void TimerSetUp()
        {
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
            //DispPoint();
        }

        public void ChangeLblText()
        {
            if (GameManager.gameMode == GameMode.Battle)
            {
                lblAttack.Text = Const.attackLabelText;
                lblDefence.Text = Const.defenceLabelText;
                lblHeal.Text = Const.healLabelText;
                lblEscape.Text = Const.escapeLabelText;
            }
            else
            {
                lblAttack.Text = Const.upMoveText;
                lblDefence.Text = Const.leftMoveText;
                lblHeal.Text = Const.rightMoveText;
                lblEscape.Text = Const.downMoveText;
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
                Image = gameManager.Map.OverrayCanvas,
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
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            this.Controls.Add(monsterImg);

            monsterImg.BringToFront();

            MsgBox.Location = new Point(10, 440);
        }//InitPictureBoxes

        public void SetMonsterImage(Image img)
        {
            monsterImg.Image = img;
            monsterImg.Location = new Point(96, 96);
            monsterImg.Size = new Size(Map.tileSize * 8, Map.tileSize * 8);
        }

        private void MainFormKeyDown(object sender, KeyEventArgs e)
        {
            SetLblCol(e.KeyCode, lblSelectCol);
            gameManager.KeyInput(e.KeyCode, mapImage, overlayImg);
        }

        private void FormKeyUp(object sender, KeyEventArgs e)
        {
            SetLblCol(e.KeyCode, lblBaseCol);
        }

        private void SetLblCol(Keys keyCode, Color color)
        {
            switch (keyCode)
            {
                case Keys.Left:
                    lblDefence.BackColor = color;
                    break;
                case Keys.Right:
                    lblHeal.BackColor = color;
                    break;
                case Keys.Up:
                    lblAttack.BackColor = color;
                    break;
                case Keys.Down:
                    lblEscape.BackColor = color;
                    break;
                default:
                    break;
            }
        }

        private void StateBoxPaint(object sender, PaintEventArgs e)
        {
            drawInfo.DrawStatus(e.Graphics, gameManager.Player);
        }

        private void MsgBoxPaint(object sender, PaintEventArgs e)
        {
            gameManager.Message.Draw(e.Graphics);
        }

        private async void lblAttackClickAsync(object sender, EventArgs e)
            => await HandleBattleOrExploreAsync(Const.CommandAtk, Keys.Up);

        private async void lblDefenceClickAsync(object sender, EventArgs e)
            => await HandleBattleOrExploreAsync(Const.CommandDef, Keys.Left);

        private async void lblHealClickAsync(object sender, EventArgs e)
            => await HandleBattleOrExploreAsync(Const.CommandHeal, Keys.Right);

        private async void lblEscapeClickAsync(object sender, EventArgs e)
            => await HandleBattleOrExploreAsync(Const.CommandEsc, Keys.Down);

        //ここは上下左右ラベルのマウスクリック・対応キー入力時に呼ばれる
        private async Task HandleBattleOrExploreAsync(string command, Keys exploreKey)
        {
            if (isBattleInputLocked && DateTime.Now < battleInputUnlockTime) return;
            if (_isWaiting) return;
            if (DateTime.Now - lastInputTime < inputCooldown) return;

            _isWaiting = true;
            lastInputTime = DateTime.Now;

            SetLblCol(exploreKey, lblSelectCol);
            await Task.Delay(100);

            if (GameManager.gameMode == GameMode.Battle)
            {
                SetLabelVisible(false);
                await gameManager.Battle.PlayerTurnAsync(command);
                await gameManager.BattleCheckAsync();
            }
            else if (GameManager.gameMode == GameMode.Explore)
            {
                gameManager.KeyInput(exploreKey, mapImage, overlayImg);
            }

            SetLabelBaseCol();

            _isWaiting = false;
        }

        public async Task ShakeAsync(int shakeTarget, int critical, int durationMs = 500, int intervalMs = 30)
        {
            PictureBox target = default!;
            if (shakeTarget == targetIsPlayer) target = MsgBox;
            else if (shakeTarget == targetIsEnemy) target = monsterImg;

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

        private void SetLabelVisible(bool isVisible)
        {
            lblAttack.Visible = isVisible;
            lblDefence.Visible = isVisible;
            lblHeal.Visible = isVisible;
            lblEscape.Visible = isVisible;
            SetLabelBaseCol();
        }

        private void SetLabelBaseCol()
        {
            lblDefence.BackColor = lblBaseCol;
            lblHeal.BackColor = lblBaseCol;
            lblAttack.BackColor = lblBaseCol;
            lblEscape.BackColor = lblBaseCol;
        }

        public void LabelHealVisible()
        {
            if (GameManager.gameMode != GameMode.Battle)
            {
                int potionCount = gameManager.Player.GetItemCount(Const.potion);
                lblHeal.Visible = potionCount > 0;
            }
        }

        private void SetMonsterImgVisible(bool visible) => monsterImg.Visible = visible;

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
            if (sender is Label lbl && itemMap.TryGetValue(lbl, out string? itemName))
            {
                UseItem(itemName);
            }
        }

        private async void UseItem(string itemName)
        {
            SetUseLabelCol(itemName, lblSelectCol);

            switch (itemName)
            {
                case Const.potion:
                    if (gameManager.Player.Hp == gameManager.Player.MaxHp)
                    {
                        gameManager.Message.Show(Const.hpFullMsg);
                        await Task.Delay(200);
                        SetUseLabelCol(itemName, lblBaseCol);
                        return;
                    }
                    gameManager.Player.Heal(30);
                    break;

                case Const.curePoison:
                    gameManager.Player.HealStatus();
                    break;

                case Const.torch:
                    Map.AddViewRadius(4);
                    break;
            }

            gameManager.Message.Show($"{itemName}を使った！");
            gameManager.Player.UseItem(itemName);
            await Task.Delay(200);
            SetUseLabelCol(itemName, lblBaseCol);
        }

        private void SetUseLabelCol(string item, Color color)
        {
            switch (item)
            {
                case Const.potion:
                    lblUsePosion.BackColor = color;
                    break;
                case Const.curePoison:
                    lblUseCurePoison.BackColor = color;
                    break;
                case Const.torch:
                    lblUseTorch.BackColor = color;
                    break;
                default:
                    break;
            }
        }

        // デバッグ用
        private void DispPoint()
        {
            label1.Text = $"XY:({Map.playerPos.X},{Map.playerPos.Y}) mi:({mapImage.Location.X},{mapImage.Location.Y}) " +
                $"oi:({overlayImg.Location.X},{overlayImg.Location.Y}) mode:({GameManager.gameMode})";
        }

    }//class
}//namespace
