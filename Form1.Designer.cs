namespace змейка
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.start = new System.Windows.Forms.Button();
            this.restart = new System.Windows.Forms.Button();
            this.BodySnaceCountText = new System.Windows.Forms.Label();
            this.FutureMovesText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(88, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(749, 423);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(13, 13);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(69, 37);
            this.start.TabIndex = 1;
            this.start.TabStop = false;
            this.start.Text = "start";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.start_Click);
            // 
            // restart
            // 
            this.restart.Location = new System.Drawing.Point(13, 56);
            this.restart.Name = "restart";
            this.restart.Size = new System.Drawing.Size(69, 37);
            this.restart.TabIndex = 2;
            this.restart.TabStop = false;
            this.restart.Text = "restart";
            this.restart.UseVisualStyleBackColor = true;
            this.restart.Click += new System.EventHandler(this.restart_Click);
            // 
            // BodySnaceCountText
            // 
            this.BodySnaceCountText.AutoSize = true;
            this.BodySnaceCountText.Location = new System.Drawing.Point(13, 100);
            this.BodySnaceCountText.Name = "BodySnaceCountText";
            this.BodySnaceCountText.Size = new System.Drawing.Size(13, 13);
            this.BodySnaceCountText.TabIndex = 3;
            this.BodySnaceCountText.Text = "4";
            // 
            // FutureMovesText
            // 
            this.FutureMovesText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.FutureMovesText.AutoSize = true;
            this.FutureMovesText.Location = new System.Drawing.Point(10, 423);
            this.FutureMovesText.Name = "FutureMovesText";
            this.FutureMovesText.Size = new System.Drawing.Size(90, 13);
            this.FutureMovesText.TabIndex = 4;
            this.FutureMovesText.Text = "FutureMovesText";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(849, 448);
            this.Controls.Add(this.FutureMovesText);
            this.Controls.Add(this.BodySnaceCountText);
            this.Controls.Add(this.restart);
            this.Controls.Add(this.start);
            this.Controls.Add(this.pictureBox1);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "змейка";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button start;
        private System.Windows.Forms.Button restart;
        private System.Windows.Forms.Label BodySnaceCountText;
        private System.Windows.Forms.Label FutureMovesText;
    }
}

