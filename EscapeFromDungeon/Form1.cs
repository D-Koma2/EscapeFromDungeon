
using EscapeFromDungeon.Properties;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EscapeFromDungeon
{
    public partial class Form1 : Form
    {
        private readonly Color btnBaseCol = Color.DarkGray;
        private readonly Color btnSelectCol = Color.DarkOrange;

        private PictureBox mapImage, overlayImg, playerImg, monsterImg;
        private GameManager gameManager;
        private DrawInfo drawInfo;

        private System.Windows.Forms.Timer timer;//画面表示更新用
        private const int timerInterval = 32;

        public static bool isBattleInputLocked = false;
        public static DateTime battleInputUnlockTime;

        private DateTime lastInputTime = DateTime.MinValue;
        private readonly TimeSpan inputCooldown = TimeSpan.FromMilliseconds(300);

        private bool isWaiting = false;

        public Form1()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            gameManager = new GameManager();//最初に生成する事!
            drawInfo = new DrawInfo();

            InitPictureBoxes();
            MsgBox.Location = new Point(10, 440);

            gameManager.Battle.SetButtonEnabled = SetBattleButtonsEnabled;
            gameManager.Battle.SetMonsterVisible = SetMonsterVisible;
            gameManager.Battle.ChangeLblText = ChangeLblText;
            gameManager.ChangeLblText = ChangeLblText;
            gameManager.KeyUpPressed = () => lblAttack_Click(this, EventArgs.Empty);
            gameManager.KeyLeftPressed = () => lblDefence_Click(this, EventArgs.Empty);
            gameManager.KeyRightPressed = () => lblHeal_Click(this, EventArgs.Empty);
            gameManager.KeyDownPressed = () => lblEscape_Click(this, EventArgs.Empty);

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
            playerImg.Image = gameManager.player.playerImage;
            gameManager.PlayerVisible(playerImg);
            overlayImg.Invalidate();
            StateBox.Invalidate();
            MsgBox.Invalidate();
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
                lblAttack.Text = "↑";
                lblDefence.Text = "←";
                lblHeal.Text = "→";
                lblEscape.Text = "↓";
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
                Location = new Point(0, 0),
                Image = gameManager.Map.MapCanvas,
                SizeMode = PictureBoxSizeMode.Normal
            };

            mapDrawBox.Controls.Add(mapImage);

            // オーバーレイ画像
            overlayImg = new PictureBox
            {
                Size = mapDrawBox.Size,
                Location = new Point(0, 0),
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
                Image = gameManager.player.playerImage,
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
                Parent = overlayImg,
                Visible = false
            };

            monsterImg.BringToFront();
        }//InitPictureBoxes

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            gameManager.KeyInput(e.KeyCode, mapImage, overlayImg);
        }

        private void StateBox_Paint(object sender, PaintEventArgs e)
        {
            drawInfo.DrawStatus(e.Graphics, gameManager.player);
        }

        private void MsgBox_Paint(object sender, PaintEventArgs e)
        {
            gameManager.Message.Draw(e.Graphics);
        }

        private async void lblAttack_Click(object sender, EventArgs e)
        {
            if (isBattleInputLocked && DateTime.Now < battleInputUnlockTime) return;
            if (isWaiting) return;
            if (DateTime.Now - lastInputTime < inputCooldown) return;

            isWaiting = true;
            lastInputTime = DateTime.Now;

            if (GameManager.gameMode == GameMode.Battle)
            {
                SetBattleButtonsEnabled(false);
                await gameManager.Battle.PlayerTurn("Attack");
                await gameManager.BattleCheck();
            }
            if (GameManager.gameMode == GameMode.Explore)
            {
                gameManager.KeyInput(Keys.Up, mapImage, overlayImg);
            }

            isWaiting = false;
        }

        private async void lblDefence_Click(object sender, EventArgs e)
        {
            if (isBattleInputLocked && DateTime.Now < battleInputUnlockTime) return;
            if (isWaiting) return;
            if (DateTime.Now - lastInputTime < inputCooldown) return;

            isWaiting = true;
            lastInputTime = DateTime.Now;

            if (GameManager.gameMode == GameMode.Battle)
            {
                SetBattleButtonsEnabled(false);
                await gameManager.Battle.PlayerTurn("Defence");
                await gameManager.BattleCheck();
            }
            if (GameManager.gameMode == GameMode.Explore)
            {
                gameManager.KeyInput(Keys.Left, mapImage, overlayImg);
            }

            isWaiting = false;
        }

        private async void lblHeal_Click(object sender, EventArgs e)
        {
            if (isBattleInputLocked && DateTime.Now < battleInputUnlockTime) return;
            if (isWaiting) return;
            if (DateTime.Now - lastInputTime < inputCooldown) return;

            isWaiting = true;
            lastInputTime = DateTime.Now;

            if (GameManager.gameMode == GameMode.Battle)
            {
                SetBattleButtonsEnabled(false);
                await gameManager.Battle.PlayerTurn("Heal");
                await gameManager.BattleCheck();
            }
            if (GameManager.gameMode == GameMode.Explore)
            {
                gameManager.KeyInput(Keys.Right, mapImage, overlayImg);
            }

            isWaiting = false;
        }

        private async void lblEscape_Click(object sender, EventArgs e)
        {
            if (isBattleInputLocked && DateTime.Now < battleInputUnlockTime) return;
            if (isWaiting) return;
            if (DateTime.Now - lastInputTime < inputCooldown) return;

            isWaiting = true;
            lastInputTime = DateTime.Now;

            if (GameManager.gameMode == GameMode.Battle)
            {
                SetBattleButtonsEnabled(false);
                await gameManager.Battle.PlayerTurn("Escape");
                await gameManager.BattleCheck();
            }
            if (GameManager.gameMode == GameMode.Explore)
            {
                gameManager.KeyInput(Keys.Down, mapImage, overlayImg);
            }

            isWaiting = false;
        }

        private void SetBattleButtonsEnabled(bool enabled)
        {
            lblAttack.Visible = enabled;
            lblDefence.Visible = enabled;
            lblHeal.Visible = enabled;
            lblEscape.Visible = enabled;
        }

        private void SetMonsterVisible(bool visible) => monsterImg.Visible = visible;

        private void LblAttack_MouseHover(object sender, EventArgs e) => lblAttack.BackColor = btnSelectCol;

        private void LblAttack_MouseLeave(object sender, EventArgs e) => lblAttack.BackColor = btnBaseCol;

        private void LblDefence_MouseHover(object sender, EventArgs e) => lblDefence.BackColor = btnSelectCol;

        private void LblDefence_MouseLeave(object sender, EventArgs e) => lblDefence.BackColor = btnBaseCol;

        private void LblHeal_MouseHover(object sender, EventArgs e) => lblHeal.BackColor = btnSelectCol;

        private void LblHeal_MouseLeave(object sender, EventArgs e) => lblHeal.BackColor = btnBaseCol;

        private void LblEscape_MouseHover(object sender, EventArgs e) => lblEscape.BackColor = btnSelectCol;

        private void LblEscape_MouseLeave(object sender, EventArgs e) => lblEscape.BackColor = btnBaseCol;

        // デバッグ用
        private void DispPoint()
        {
            label1.Text = $"XY:({Map.playerPos.X}, {Map.playerPos.Y}) limit:{gameManager.player.Limit} " +
                $"mi:({mapImage.Location.X},{mapImage.Location.Y}) " +
                $"oi:({overlayImg.Location.X},{overlayImg.Location.Y}) pi:({playerImg.Location.X},{playerImg.Location.Y})";
        }

    }//class
}//namespace
