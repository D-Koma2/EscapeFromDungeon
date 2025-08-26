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
            mainPicture = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)mainPicture).BeginInit();
            SuspendLayout();
            // 
            // mainPicture
            // 
            mainPicture.BackColor = Color.Gray;
            mainPicture.Location = new Point(42, 26);
            mainPicture.Name = "mainPicture";
            mainPicture.Size = new Size(486, 337);
            mainPicture.TabIndex = 0;
            mainPicture.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(792, 514);
            Controls.Add(mainPicture);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)mainPicture).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox mainPicture;
    }
}
