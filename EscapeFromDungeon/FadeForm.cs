using EscapeFromDungeon;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsAppTest2
{

    public partial class FadeForm : Form
    {
        private System.Windows.Forms.Timer fadeTimer;
        public enum FadeDir { FadeIn, FadeOut }
        private FadeDir fadeDirection;

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

            fadeTimer = new System.Windows.Forms.Timer { Interval = 16 };
            fadeTimer.Tick += FadeTimer_Tick;
        }

        public void StartFade(FadeDir direction)
        {
            this.fadeDirection = direction;
            if (this.fadeDirection == FadeDir.FadeIn) { this.Opacity = 0.0; }
            else { this.Opacity = 1.0; }
            fadeTimer.Start();
            this.Show();
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (fadeDirection == FadeDir.FadeOut)
            {
                if (this.Opacity > 0.0) { this.Opacity -= 0.01; }
                else
                {
                    fadeTimer.Stop();

                    if (GameManager.gameMode == GameMode.Title)
                    {
                        GameManager.gameMode = GameMode.Explore;
                        TitleLbl.Visible = false;
                        this.Hide();
                    }
                }
            }
            else
            {
                if (this.Opacity < 1.0) { this.Opacity += 0.02; }
                else
                {
                    fadeTimer.Stop();

                    if (GameManager.gameMode == GameMode.Title)
                    {
                        GameManager.gameMode = GameMode.Explore;
                        this.Hide();
                    }
                    else if (GameManager.gameMode == GameMode.Gameover)
                    {
                        TitleLbl.Text = "Game Over";
                        TitleLbl.Visible = true;
                        //StartBtn.Visible = true;
                        ExitBtn.Visible = true;
                    }
                }
            }
        }

        public void FollowOwner()
        {
            if (this.Owner != null)
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

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartBtn.Visible = false;
            ExitBtn.Visible = false;
            StartFade(FadeDir.FadeOut);
        }
    }
}
