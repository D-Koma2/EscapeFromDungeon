namespace EscapeFromDungeon
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            mapDrawBox = new PictureBox();
            StateBox = new PictureBox();
            label1 = new Label();
            MsgBox = new PictureBox();
            lblAttack = new Label();
            lblHeal = new Label();
            lblDefence = new Label();
            lblEscape = new Label();
            ((System.ComponentModel.ISupportInitialize)mapDrawBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StateBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MsgBox).BeginInit();
            SuspendLayout();
            // 
            // mapDrawBox
            // 
            mapDrawBox.Anchor = AnchorStyles.None;
            mapDrawBox.BackColor = Color.Black;
            mapDrawBox.BackgroundImageLayout = ImageLayout.None;
            mapDrawBox.Location = new Point(11, 11);
            mapDrawBox.Name = "mapDrawBox";
            mapDrawBox.Size = new Size(416, 416);
            mapDrawBox.TabIndex = 0;
            mapDrawBox.TabStop = false;
            // 
            // StateBox
            // 
            StateBox.Location = new Point(454, 17);
            StateBox.Name = "StateBox";
            StateBox.Size = new Size(230, 214);
            StateBox.TabIndex = 1;
            StateBox.TabStop = false;
            StateBox.Paint += StateBox_Paint;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.White;
            label1.Location = new Point(-1, -1);
            label1.Name = "label1";
            label1.Size = new Size(26, 20);
            label1.TabIndex = 2;
            label1.Text = "XY";
            // 
            // MsgBox
            // 
            MsgBox.Anchor = AnchorStyles.None;
            MsgBox.BackColor = Color.FromArgb(64, 64, 64);
            MsgBox.Location = new Point(11, 442);
            MsgBox.Name = "MsgBox";
            MsgBox.Size = new Size(477, 129);
            MsgBox.TabIndex = 3;
            MsgBox.TabStop = false;
            MsgBox.Paint += MsgBox_Paint;
            // 
            // lblAttack
            // 
            lblAttack.Anchor = AnchorStyles.None;
            lblAttack.BackColor = Color.Gray;
            lblAttack.BorderStyle = BorderStyle.Fixed3D;
            lblAttack.Location = new Point(517, 451);
            lblAttack.Name = "lblAttack";
            lblAttack.Size = new Size(69, 37);
            lblAttack.TabIndex = 4;
            lblAttack.Text = "攻撃";
            lblAttack.TextAlign = ContentAlignment.MiddleCenter;
            lblAttack.Visible = false;
            lblAttack.Click += lblAttack_Click;
            lblAttack.MouseLeave += LblAttack_MouseLeave;
            lblAttack.MouseHover += LblAttack_MouseHover;
            // 
            // lblHeal
            // 
            lblHeal.Anchor = AnchorStyles.None;
            lblHeal.BackColor = Color.Gray;
            lblHeal.BorderStyle = BorderStyle.Fixed3D;
            lblHeal.Location = new Point(612, 451);
            lblHeal.Name = "lblHeal";
            lblHeal.Size = new Size(69, 37);
            lblHeal.TabIndex = 5;
            lblHeal.Text = "回復";
            lblHeal.TextAlign = ContentAlignment.MiddleCenter;
            lblHeal.Visible = false;
            lblHeal.Click += lblHeal_Click;
            lblHeal.MouseLeave += LblHeal_MouseLeave;
            lblHeal.MouseHover += LblHeal_MouseHover;
            // 
            // lblDefence
            // 
            lblDefence.Anchor = AnchorStyles.None;
            lblDefence.BackColor = Color.Gray;
            lblDefence.BorderStyle = BorderStyle.Fixed3D;
            lblDefence.Location = new Point(517, 499);
            lblDefence.Name = "lblDefence";
            lblDefence.Size = new Size(69, 41);
            lblDefence.TabIndex = 6;
            lblDefence.Text = "防御";
            lblDefence.TextAlign = ContentAlignment.MiddleCenter;
            lblDefence.Visible = false;
            lblDefence.Click += lblDefence_Click;
            lblDefence.MouseLeave += LblDefence_MouseLeave;
            lblDefence.MouseHover += LblDefence_MouseHover;
            // 
            // lblEscape
            // 
            lblEscape.Anchor = AnchorStyles.None;
            lblEscape.BackColor = Color.Gray;
            lblEscape.BorderStyle = BorderStyle.Fixed3D;
            lblEscape.Location = new Point(612, 499);
            lblEscape.Name = "lblEscape";
            lblEscape.Size = new Size(69, 41);
            lblEscape.TabIndex = 7;
            lblEscape.Text = "逃げる";
            lblEscape.TextAlign = ContentAlignment.MiddleCenter;
            lblEscape.Visible = false;
            lblEscape.Click += lblEscape_Click;
            lblEscape.MouseLeave += LblEscape_MouseLeave;
            lblEscape.MouseHover += LblEscape_MouseHover;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(782, 553);
            Controls.Add(lblEscape);
            Controls.Add(lblDefence);
            Controls.Add(lblHeal);
            Controls.Add(lblAttack);
            Controls.Add(label1);
            Controls.Add(MsgBox);
            Controls.Add(StateBox);
            Controls.Add(mapDrawBox);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Form1";
            Text = "EscapeFromDungeon";
            KeyDown += MainForm_KeyDown;
            ((System.ComponentModel.ISupportInitialize)mapDrawBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)StateBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)MsgBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox mapDrawBox;
        private PictureBox StateBox;
        private Label label1;
        private PictureBox MsgBox;
        private Label lblAttack;
        private Label lblHeal;
        private Label lblDefence;
        private Label lblEscape;
    }
}
