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
            ((System.ComponentModel.ISupportInitialize)mapDrawBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StateBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MsgBox).BeginInit();
            SuspendLayout();
            // 
            // mapDrawBox
            // 
            mapDrawBox.Anchor = AnchorStyles.None;
            mapDrawBox.BackColor = Color.Black;
            mapDrawBox.Location = new Point(20, 17);
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
            MsgBox.Anchor = AnchorStyles.Bottom;
            MsgBox.BackColor = Color.FromArgb(64, 64, 64);
            MsgBox.Location = new Point(20, 450);
            MsgBox.Name = "MsgBox";
            MsgBox.Size = new Size(477, 129);
            MsgBox.TabIndex = 3;
            MsgBox.TabStop = false;
            MsgBox.Paint += MsgBox_Paint;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(781, 551);
            Controls.Add(label1);
            Controls.Add(MsgBox);
            Controls.Add(StateBox);
            Controls.Add(mapDrawBox);
            KeyPreview = true;
            Name = "Form1";
            Text = "Form1";
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
    }
}
