namespace WindowsFormsAppTest2
{
    partial class FadeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            StartBtn = new Button();
            ExitBtn = new Button();
            TitleLbl = new Label();
            SuspendLayout();
            // 
            // StartBtn
            // 
            StartBtn.Anchor = AnchorStyles.None;
            StartBtn.AutoSize = true;
            StartBtn.Cursor = Cursors.Hand;
            StartBtn.Location = new Point(340, 316);
            StartBtn.Name = "StartBtn";
            StartBtn.Size = new Size(95, 30);
            StartBtn.TabIndex = 0;
            StartBtn.TabStop = false;
            StartBtn.Text = "ゲームスタート";
            StartBtn.UseVisualStyleBackColor = true;
            StartBtn.Click += StartButtonClick;
            // 
            // ExitBtn
            // 
            ExitBtn.Anchor = AnchorStyles.None;
            ExitBtn.AutoSize = true;
            ExitBtn.Cursor = Cursors.Hand;
            ExitBtn.Location = new Point(340, 363);
            ExitBtn.Name = "ExitBtn";
            ExitBtn.Size = new Size(95, 30);
            ExitBtn.TabIndex = 1;
            ExitBtn.TabStop = false;
            ExitBtn.Text = "終了する";
            ExitBtn.UseVisualStyleBackColor = true;
            ExitBtn.Click += ExitButtonClick;
            // 
            // TitleLbl
            // 
            TitleLbl.Anchor = AnchorStyles.None;
            TitleLbl.AutoSize = true;
            TitleLbl.Font = new Font("Impact", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            TitleLbl.ForeColor = Color.SlateBlue;
            TitleLbl.Location = new Point(265, 41);
            TitleLbl.Name = "TitleLbl";
            TitleLbl.Size = new Size(258, 225);
            TitleLbl.TabIndex = 2;
            TitleLbl.Text = "Escape \r\nFrom \r\nDungeon";
            TitleLbl.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FadeForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.Black;
            ClientSize = new Size(800, 450);
            ControlBox = false;
            Controls.Add(TitleLbl);
            Controls.Add(ExitBtn);
            Controls.Add(StartBtn);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FadeForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "FadeForm";
            TopMost = true;
            Shown += FadeForm_Shown;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Button StartBtn;
        private Button ExitBtn;
        private Label TitleLbl;
    }
}