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
            MessageBox = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)mapDrawBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)StateBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MessageBox).BeginInit();
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
            StateBox.Location = new Point(539, 26);
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
            // MessageBox
            // 
            MessageBox.Anchor = AnchorStyles.Bottom;
            MessageBox.BackColor = Color.FromArgb(64, 64, 64);
            MessageBox.Location = new Point(20, 450);
            MessageBox.Name = "MessageBox";
            MessageBox.Size = new Size(477, 129);
            MessageBox.TabIndex = 3;
            MessageBox.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(781, 551);
            Controls.Add(label1);
            Controls.Add(MessageBox);
            Controls.Add(StateBox);
            Controls.Add(mapDrawBox);
            KeyPreview = true;
            Name = "Form1";
            Text = "Form1";
            KeyDown += MainForm_KeyDown;
            ((System.ComponentModel.ISupportInitialize)mapDrawBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)StateBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)MessageBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox mapDrawBox;
        private PictureBox StateBox;
        private Label label1;
        private PictureBox MessageBox;
    }
}
