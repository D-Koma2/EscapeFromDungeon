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
            ((System.ComponentModel.ISupportInitialize)mapDrawBox).BeginInit();
            SuspendLayout();
            // 
            // mapDrawBox
            // 
            mapDrawBox.Anchor = AnchorStyles.None;
            mapDrawBox.BackColor = Color.Black;
            mapDrawBox.Location = new Point(36, 26);
            mapDrawBox.Name = "mapDrawBox";
            mapDrawBox.Size = new Size(486, 337);
            mapDrawBox.TabIndex = 0;
            mapDrawBox.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(781, 514);
            Controls.Add(mapDrawBox);
            KeyPreview = true;
            Name = "Form1";
            Text = "Form1";
            KeyDown += MainForm_KeyDown;
            ((System.ComponentModel.ISupportInitialize)mapDrawBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox mapDrawBox;
    }
}
