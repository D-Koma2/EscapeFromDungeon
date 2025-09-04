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
            lblUsePosion = new Label();
            lblUseCurePoison = new Label();
            lblUseTorch = new Label();
            LimitBox = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)mapDrawBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StateBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MsgBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)LimitBox).BeginInit();
            SuspendLayout();
            // 
            // mapDrawBox
            // 
            mapDrawBox.Anchor = AnchorStyles.None;
            mapDrawBox.BackColor = Color.Black;
            mapDrawBox.BackgroundImageLayout = ImageLayout.None;
            mapDrawBox.BorderStyle = BorderStyle.FixedSingle;
            mapDrawBox.Location = new Point(12, 12);
            mapDrawBox.Name = "mapDrawBox";
            mapDrawBox.Size = new Size(416, 416);
            mapDrawBox.TabIndex = 0;
            mapDrawBox.TabStop = false;
            // 
            // StateBox
            // 
            StateBox.BackColor = Color.FromArgb(0, 0, 64);
            StateBox.BorderStyle = BorderStyle.FixedSingle;
            StateBox.Location = new Point(488, 12);
            StateBox.Name = "StateBox";
            StateBox.Size = new Size(282, 416);
            StateBox.TabIndex = 1;
            StateBox.TabStop = false;
            StateBox.Paint += StateBoxPaint;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(0, 0, 64);
            label1.ForeColor = Color.White;
            label1.Location = new Point(21, 516);
            label1.Name = "label1";
            label1.Size = new Size(26, 20);
            label1.TabIndex = 2;
            label1.Text = "XY";
            label1.Visible = false;
            // 
            // MsgBox
            // 
            MsgBox.Anchor = AnchorStyles.None;
            MsgBox.BackColor = Color.FromArgb(0, 0, 64);
            MsgBox.BorderStyle = BorderStyle.FixedSingle;
            MsgBox.Location = new Point(11, 439);
            MsgBox.Name = "MsgBox";
            MsgBox.Size = new Size(506, 102);
            MsgBox.TabIndex = 3;
            MsgBox.TabStop = false;
            MsgBox.Paint += MsgBoxPaint;
            // 
            // lblAttack
            // 
            lblAttack.Anchor = AnchorStyles.None;
            lblAttack.BackColor = Color.DarkGray;
            lblAttack.BorderStyle = BorderStyle.FixedSingle;
            lblAttack.Cursor = Cursors.Hand;
            lblAttack.Location = new Point(609, 445);
            lblAttack.Name = "lblAttack";
            lblAttack.Size = new Size(80, 40);
            lblAttack.TabIndex = 4;
            lblAttack.Text = "[↑] 攻撃";
            lblAttack.TextAlign = ContentAlignment.MiddleCenter;
            lblAttack.Click += lblAttackClickAsync;
            // 
            // lblHeal
            // 
            lblHeal.Anchor = AnchorStyles.None;
            lblHeal.BackColor = Color.DarkGray;
            lblHeal.BorderStyle = BorderStyle.FixedSingle;
            lblHeal.Cursor = Cursors.Hand;
            lblHeal.Location = new Point(695, 469);
            lblHeal.Name = "lblHeal";
            lblHeal.Size = new Size(80, 40);
            lblHeal.TabIndex = 5;
            lblHeal.Text = "[→] 回復";
            lblHeal.TextAlign = ContentAlignment.MiddleCenter;
            lblHeal.Click += lblHealClickAsync;
            // 
            // lblDefence
            // 
            lblDefence.Anchor = AnchorStyles.None;
            lblDefence.BackColor = Color.DarkGray;
            lblDefence.BorderStyle = BorderStyle.FixedSingle;
            lblDefence.Cursor = Cursors.Hand;
            lblDefence.Location = new Point(523, 469);
            lblDefence.Name = "lblDefence";
            lblDefence.Size = new Size(80, 40);
            lblDefence.TabIndex = 6;
            lblDefence.Text = "[←] 防御";
            lblDefence.TextAlign = ContentAlignment.MiddleCenter;
            lblDefence.Click += lblDefenceClickAsync;
            // 
            // lblEscape
            // 
            lblEscape.Anchor = AnchorStyles.None;
            lblEscape.BackColor = Color.DarkGray;
            lblEscape.BorderStyle = BorderStyle.FixedSingle;
            lblEscape.Cursor = Cursors.Hand;
            lblEscape.Location = new Point(609, 496);
            lblEscape.Name = "lblEscape";
            lblEscape.Size = new Size(80, 40);
            lblEscape.TabIndex = 7;
            lblEscape.Text = "[↓] 逃げる";
            lblEscape.TextAlign = ContentAlignment.MiddleCenter;
            lblEscape.Click += lblEscapeClickAsync;
            // 
            // lblUsePosion
            // 
            lblUsePosion.Anchor = AnchorStyles.None;
            lblUsePosion.BackColor = Color.DarkGray;
            lblUsePosion.Cursor = Cursors.Hand;
            lblUsePosion.Location = new Point(497, 95);
            lblUsePosion.Name = "lblUsePosion";
            lblUsePosion.Size = new Size(62, 21);
            lblUsePosion.TabIndex = 8;
            lblUsePosion.Text = "[p] 使う";
            lblUsePosion.TextAlign = ContentAlignment.MiddleCenter;
            lblUsePosion.Click += ItemLabelClick;
            // 
            // lblUseCurePoison
            // 
            lblUseCurePoison.Anchor = AnchorStyles.None;
            lblUseCurePoison.BackColor = Color.DarkGray;
            lblUseCurePoison.Cursor = Cursors.Hand;
            lblUseCurePoison.Location = new Point(497, 125);
            lblUseCurePoison.Name = "lblUseCurePoison";
            lblUseCurePoison.Size = new Size(62, 20);
            lblUseCurePoison.TabIndex = 9;
            lblUseCurePoison.Text = "[O] 使う";
            lblUseCurePoison.TextAlign = ContentAlignment.MiddleCenter;
            lblUseCurePoison.Click += ItemLabelClick;
            // 
            // lblUseTorch
            // 
            lblUseTorch.Anchor = AnchorStyles.None;
            lblUseTorch.BackColor = Color.DarkGray;
            lblUseTorch.Cursor = Cursors.Hand;
            lblUseTorch.Location = new Point(497, 155);
            lblUseTorch.Name = "lblUseTorch";
            lblUseTorch.Size = new Size(62, 23);
            lblUseTorch.TabIndex = 10;
            lblUseTorch.Text = "[ I ] 使う";
            lblUseTorch.TextAlign = ContentAlignment.MiddleCenter;
            lblUseTorch.Click += ItemLabelClick;
            // 
            // LimitBox
            // 
            LimitBox.Anchor = AnchorStyles.None;
            LimitBox.Location = new Point(434, 12);
            LimitBox.Name = "LimitBox";
            LimitBox.Size = new Size(48, 416);
            LimitBox.TabIndex = 11;
            LimitBox.TabStop = false;
            LimitBox.Paint += LimitBoxPaint;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(0, 0, 32);
            ClientSize = new Size(782, 553);
            Controls.Add(LimitBox);
            Controls.Add(lblUseTorch);
            Controls.Add(lblUseCurePoison);
            Controls.Add(lblUsePosion);
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
            KeyDown += MainFormKeyDown;
            KeyUp += FormKeyUp;
            ((System.ComponentModel.ISupportInitialize)mapDrawBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)StateBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)MsgBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)LimitBox).EndInit();
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
        private Label lblUsePosion;
        private Label lblUseCurePoison;
        private Label lblUseTorch;
        private PictureBox LimitBox;
    }
}
