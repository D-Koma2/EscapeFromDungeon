using EscapeFromDungeon.Constants;

namespace EscapeFromDungeon.Core
{
    public partial class FadeForm : Form
    {
        private System.Windows.Forms.Timer fadeTimer;
        private const int timerInterval = 16;
        public enum FadeDir { FadeIn, FadeOut }
        private FadeDir fadeDirection;

        public Action? InitStart;

        public FadeForm(Form mainForm, FadeDir fadeDirection)
        {
            InitializeComponent();
            Rectangle bounds = mainForm.Bounds;
            //自動配置を無効
            this.StartPosition = FormStartPosition.Manual;

            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.BackColor = Color.Black;
            this.AutoScaleMode = AutoScaleMode.None;

            // MainForm の上に表示
            this.Owner = mainForm;
            FollowOwner();// 手動で位置指定

            StartBtn.Text = Const.startMenu;
            ExitBtn.Text = Const.exitMenu;

            fadeTimer = new System.Windows.Forms.Timer { Interval = timerInterval };
            fadeTimer.Tick += FadeTimer_Tick;

            GameManager.bgmPlayer.PlayLoop(Properties.Resources.maou_bgm_8bit06);
        }

        public void StartFade(FadeDir direction)
        {
            CautionLbl.Visible = false;
            this.fadeDirection = direction;

            if (this.fadeDirection is FadeDir.FadeIn) { this.Opacity = 0.0; }
            else { this.Opacity = 1.0; }

            fadeTimer.Start();
            this.Show();

            if(GameStateManager.Instance.CurrentMode is GameMode.Title)
                GameManager.bgmPlayer.PlayLoop(Properties.Resources.maou_bgm_8bit04);
        }

        private void FadeTimer_Tick(object? sender, EventArgs e)
        {
            if (fadeDirection is FadeDir.FadeOut)
            {
                if (this.Opacity > 0.0) { this.Opacity -= 0.01; }
                else
                {
                    fadeTimer.Stop();

                    if (GameStateManager.Instance.CurrentMode is GameMode.Title)
                    {
                        GameStateManager.Instance.ChangeMode(GameMode.Explore);
                        TitleLbl.Visible = false;
                        this.Hide();
                    }
                }
            }
            else
            {
                if (this.Opacity < 1.0) { this.Opacity += 0.01; }
                else
                {
                    fadeTimer.Stop();

                    if (GameStateManager.Instance.CurrentMode is GameMode.Title)
                    {
                        GameStateManager.Instance.ChangeMode(GameMode.Explore);
                        this.Hide();
                    }
                    else if (GameStateManager.Instance.CurrentMode is (GameMode.Gameover or GameMode.GameClear))
                    {
                        TitleLbl.Text = GameStateManager.Instance.CurrentMode is GameMode.Gameover ? Const.gameOver : Const.gameClear;
                        this.KeyPreview = true;
                        StartBtn.Text = Const.retry;
                        TitleLbl.Visible = true;
                        StartBtn.Visible = true;
                        ExitBtn.Visible = true;
                        GameStateManager.Instance.ChangeMode(GameMode.Reset);
                    }
                }
            }
        }

        public void FollowOwner()
        {
            if (this.Owner is not null)
            {
                //メインフォームのクライアント領域の左上をスクリーン座標に変換
                this.Location = Owner.PointToScreen(Point.Empty);
                //枠やタイトルバーを除いた内部サイズを取得
                this.Size = Owner.ClientSize;

                //枠やタイトルバーも含めて覆いたい場合は以下のように設定
                //this.Location = mainForm.DesktopBounds.Location;
                //this.Size = mainForm.DesktopBounds.Size;
            }
        }

        private void StartButtonClick(object sender, EventArgs e)
        {
            StartBtn.Visible = false;
            ExitBtn.Visible = false;
            TitleLbl.Visible = false;
            InitStart?.Invoke();
            StartFade(FadeDir.FadeOut);
        }

        private void ExitButtonClick(object sender, EventArgs e) => Application.Exit();

        private void FadeForm_Show(object sender, EventArgs e)
        {
            this.Activate();
            this.KeyPreview = true;
            this.Focus();
            StartBtn.Focus();
        }
    }
}
