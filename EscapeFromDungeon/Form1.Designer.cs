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
            mapPicture = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)mainPicture).BeginInit();
            ((System.ComponentModel.ISupportInitialize)mapPicture).BeginInit();
            SuspendLayout();
            // 
            // mainPicture
            // 
            mainPicture.Location = new Point(42, 26);
            mainPicture.Name = "mainPicture";
            mainPicture.Size = new Size(486, 337);
            mainPicture.TabIndex = 0;
            mainPicture.TabStop = false;
            // 
            // mapPicture
            // 
            mapPicture.Location = new Point(122, 76);
            mapPicture.Name = "mapPicture";
            mapPicture.Size = new Size(351, 222);
            mapPicture.TabIndex = 1;
            mapPicture.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(792, 514);
            Controls.Add(mapPicture);
            Controls.Add(mainPicture);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)mainPicture).EndInit();
            ((System.ComponentModel.ISupportInitialize)mapPicture).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox mainPicture;
        private PictureBox mapPicture;
    }
}
