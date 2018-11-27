namespace ml_csharp_lesson1
{
    partial class MainForm
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
            this.videoPlayer = new Accord.Controls.VideoSourcePlayer();
            this.edgeBox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.trafficSignBox = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this._currentFrameMatchesTextBox = new System.Windows.Forms.TextBox();
            this._totalFrameMatchesTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.edgeBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trafficSignBox)).BeginInit();
            this.SuspendLayout();
            // 
            // videoPlayer
            // 
            this.videoPlayer.Location = new System.Drawing.Point(7, 6);
            this.videoPlayer.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.videoPlayer.Name = "videoPlayer";
            this.videoPlayer.Size = new System.Drawing.Size(731, 530);
            this.videoPlayer.TabIndex = 0;
            this.videoPlayer.Text = "videoSourcePlayer1";
            this.videoPlayer.VideoSource = null;
            this.videoPlayer.NewFrame += new Accord.Controls.VideoSourcePlayer.NewFrameHandler(this.VideoPlayer_NewFrame);
            // 
            // edgeBox
            // 
            this.edgeBox.BackColor = System.Drawing.Color.Black;
            this.edgeBox.Location = new System.Drawing.Point(7, 540);
            this.edgeBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.edgeBox.Name = "edgeBox";
            this.edgeBox.Size = new System.Drawing.Size(366, 221);
            this.edgeBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.edgeBox.TabIndex = 1;
            this.edgeBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(14, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(258, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Front Facing Camera - Raw Feed";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(14, 546);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Edge Detection";
            // 
            // trafficSignBox
            // 
            this.trafficSignBox.BackColor = System.Drawing.Color.Black;
            this.trafficSignBox.Location = new System.Drawing.Point(373, 540);
            this.trafficSignBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.trafficSignBox.Name = "trafficSignBox";
            this.trafficSignBox.Size = new System.Drawing.Size(366, 221);
            this.trafficSignBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.trafficSignBox.TabIndex = 5;
            this.trafficSignBox.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(381, 546);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(172, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Traffic Sign Detection";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(369, 777);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Frame Matchs:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(369, 813);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 20);
            this.label5.TabIndex = 8;
            this.label5.Text = "Total Matches:";
            // 
            // _frameMatchesTextBox
            // 
            this._currentFrameMatchesTextBox.Location = new System.Drawing.Point(505, 776);
            this._currentFrameMatchesTextBox.Name = "_frameMatchesTextBox";
            this._currentFrameMatchesTextBox.ReadOnly = true;
            this._currentFrameMatchesTextBox.Size = new System.Drawing.Size(117, 22);
            this._currentFrameMatchesTextBox.TabIndex = 9;
            // 
            // _totalFrameMatchesTextBox
            // 
            this._totalFrameMatchesTextBox.Location = new System.Drawing.Point(505, 811);
            this._totalFrameMatchesTextBox.Name = "_totalFrameMatchesTextBox";
            this._totalFrameMatchesTextBox.ReadOnly = true;
            this._totalFrameMatchesTextBox.Size = new System.Drawing.Size(117, 22);
            this._totalFrameMatchesTextBox.TabIndex = 10;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(746, 856);
            this.Controls.Add(this._totalFrameMatchesTextBox);
            this.Controls.Add(this._currentFrameMatchesTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.trafficSignBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.edgeBox);
            this.Controls.Add(this.videoPlayer);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "The Machine Learning Advantage Course";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.edgeBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trafficSignBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Accord.Controls.VideoSourcePlayer videoPlayer;
        private System.Windows.Forms.PictureBox edgeBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox trafficSignBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox _currentFrameMatchesTextBox;
        private System.Windows.Forms.TextBox _totalFrameMatchesTextBox;
    }
}

