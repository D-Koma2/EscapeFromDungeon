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
            mapDrawBox.BorderStyle = BorderStyle.FixedSingle;
            mapDrawBox.Location = new Point(12, 12);
            mapDrawBox.Name = "mapDrawBox";
            mapDrawBox.Size = new Size(416, 416);
            mapDrawBox.TabIndex = 0;
            mapDrawBox.TabStop = false;
            // 
            // StateBox
            // 
            StateBox.BorderStyle = BorderStyle.FixedSingle;
            StateBox.Location = new Point(444, 12);
            StateBox.Name = "StateBox";
            StateBox.Size = new Size(301, 410);
            StateBox.TabIndex = 1;
            StateBox.TabStop = false;
            StateBox.Paint += StateBoxPaint;
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
            MsgBox.BackColor = Color.Indigo;
            MsgBox.BorderStyle = BorderStyle.FixedSingle;
            MsgBox.Location = new Point(11, 439);
            MsgBox.Name = "MsgBox";
            MsgBox.Size = new Size(477, 102);
            MsgBox.TabIndex = 3;
            MsgBox.TabStop = false;
            MsgBox.Paint += MsgBoxPaint;
            // 
            // lblAttack
            // 
            lblAttack.Anchor = AnchorStyles.None;
            lblAttack.BackColor = Color.Gray;
            lblAttack.BorderStyle = BorderStyle.FixedSingle;
            lblAttack.Cursor = Cursors.Hand;
            lblAttack.Location = new Point(584, 439);
            lblAttack.Name = "lblAttack";
            lblAttack.Size = new Size(69, 37);
            lblAttack.TabIndex = 4;
            lblAttack.Text = "攻撃";
            lblAttack.TextAlign = ContentAlignment.MiddleCenter;
            lblAttack.Click += lblAttackClickAsynk;
            lblAttack.MouseLeave += LabelMouseLeave;
            lblAttack.MouseHover += LabelMouseHover;
            // 
            // lblHeal
            // 
            lblHeal.Anchor = AnchorStyles.None;
            lblHeal.BackColor = Color.Gray;
            lblHeal.BorderStyle = BorderStyle.FixedSingle;
            lblHeal.Cursor = Cursors.Hand;
            lblHeal.Location = new Point(659, 463);
            lblHeal.Name = "lblHeal";
            lblHeal.Size = new Size(69, 37);
            lblHeal.TabIndex = 5;
            lblHeal.Text = "回復";
            lblHeal.TextAlign = ContentAlignment.MiddleCenter;
            lblHeal.Click += lblHealClickAsync;
            lblHeal.MouseLeave += LabelMouseLeave;
            lblHeal.MouseHover += LabelMouseHover;
            // 
            // lblDefence
            // 
            lblDefence.Anchor = AnchorStyles.None;
            lblDefence.BackColor = Color.Gray;
            lblDefence.BorderStyle = BorderStyle.FixedSingle;
            lblDefence.Cursor = Cursors.Hand;
            lblDefence.Location = new Point(509, 463);
            lblDefence.Name = "lblDefence";
            lblDefence.Size = new Size(69, 41);
            lblDefence.TabIndex = 6;
            lblDefence.Text = "防御";
            lblDefence.TextAlign = ContentAlignment.MiddleCenter;
            lblDefence.Click += lblDefenceClickAsync;
            lblDefence.MouseLeave += LabelMouseLeave;
            lblDefence.MouseHover += LabelMouseHover;
            // 
            // lblEscape
            // 
            lblEscape.Anchor = AnchorStyles.None;
            lblEscape.BackColor = Color.Gray;
            lblEscape.BorderStyle = BorderStyle.FixedSingle;
            lblEscape.Cursor = Cursors.Hand;
            lblEscape.Location = new Point(584, 490);
            lblEscape.Name = "lblEscape";
            lblEscape.Size = new Size(69, 41);
            lblEscape.TabIndex = 7;
            lblEscape.Text = "逃げる";
            lblEscape.TextAlign = ContentAlignment.MiddleCenter;
            lblEscape.Click += lblEscapeClickAsync;
            lblEscape.MouseLeave += LabelMouseLeave;
            lblEscape.MouseHover += LabelMouseHover;
            // 
            // lblUsePosion
            // 
            lblUsePosion.Anchor = AnchorStyles.None;
            lblUsePosion.BackColor = Color.DimGray;
            lblUsePosion.Cursor = Cursors.Hand;
            lblUsePosion.Location = new Point(467, 96);
            lblUsePosion.Name = "lblUsePosion";
            lblUsePosion.Size = new Size(62, 21);
            lblUsePosion.TabIndex = 8;
            lblUsePosion.Text = "使う";
            lblUsePosion.TextAlign = ContentAlignment.MiddleCenter;
            lblUsePosion.Click += ItemLabelClick;
            // 
            // lblUseCurePoison
            // 
            lblUseCurePoison.Anchor = AnchorStyles.None;
            lblUseCurePoison.BackColor = Color.DimGray;
            lblUseCurePoison.Cursor = Cursors.Hand;
            lblUseCurePoison.Location = new Point(467, 126);
            lblUseCurePoison.Name = "lblUseCurePoison";
            lblUseCurePoison.Size = new Size(62, 20);
            lblUseCurePoison.TabIndex = 9;
            lblUseCurePoison.Text = "使う";
            lblUseCurePoison.TextAlign = ContentAlignment.MiddleCenter;
            lblUseCurePoison.Click += ItemLabelClick;
            // 
            // lblUseTorch
            // 
            lblUseTorch.Anchor = AnchorStyles.None;
            lblUseTorch.BackColor = Color.DimGray;
            lblUseTorch.Cursor = Cursors.Hand;
            lblUseTorch.Location = new Point(467, 156);
            lblUseTorch.Name = "lblUseTorch";
            lblUseTorch.Size = new Size(62, 23);
            lblUseTorch.TabIndex = 10;
            lblUseTorch.Text = "使う";
            lblUseTorch.TextAlign = ContentAlignment.MiddleCenter;
            lblUseTorch.Click += ItemLabelClick;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(782, 553);
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
        private Label lblUsePosion;
        private Label lblUseCurePoison;
        private Label lblUseTorch;
    }
}
