namespace ml_csharp_lesson3
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
            this.components = new System.ComponentModel.Container();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.videoPlayer = new Accord.Controls.VideoSourcePlayer();
            this.autopilotLabel = new System.Windows.Forms.Label();
            this.carBox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.headBox = new System.Windows.Forms.PictureBox();
            this.eyeBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.carBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.headBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eyeBox)).BeginInit();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(573, 808);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(271, 31);
            this.label3.TabIndex = 7;
            this.label3.Text = "Head Pose Detection";
            // 
            // videoPlayer
            // 
            this.videoPlayer.Location = new System.Drawing.Point(974, 12);
            this.videoPlayer.Name = "videoPlayer";
            this.videoPlayer.Size = new System.Drawing.Size(133, 130);
            this.videoPlayer.TabIndex = 12;
            this.videoPlayer.Text = "videoSourcePlayer1";
            this.videoPlayer.VideoSource = null;
            this.videoPlayer.NewFrameReceived += new Accord.Video.NewFrameEventHandler(this.videoPlayer_NewFrameReceived);
            // 
            // autopilotLabel
            // 
            this.autopilotLabel.AutoSize = true;
            this.autopilotLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.autopilotLabel.ForeColor = System.Drawing.Color.Red;
            this.autopilotLabel.Location = new System.Drawing.Point(56, 637);
            this.autopilotLabel.Name = "autopilotLabel";
            this.autopilotLabel.Size = new System.Drawing.Size(1005, 55);
            this.autopilotLabel.TabIndex = 17;
            this.autopilotLabel.Text = "WARNING - AUTOPILOT WILL DISENGAGE!";
            this.autopilotLabel.Visible = false;
            // 
            // carBox
            // 
            this.carBox.Location = new System.Drawing.Point(10, 10);
            this.carBox.Name = "carBox";
            this.carBox.Size = new System.Drawing.Size(1097, 780);
            this.carBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.carBox.TabIndex = 18;
            this.carBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(26, 808);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(255, 31);
            this.label1.TabIndex = 19;
            this.label1.Text = "Eye State Detection";
            // 
            // headBox
            // 
            this.headBox.BackColor = System.Drawing.Color.Black;
            this.headBox.Location = new System.Drawing.Point(572, 808);
            this.headBox.Name = "headBox";
            this.headBox.Size = new System.Drawing.Size(535, 390);
            this.headBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.headBox.TabIndex = 20;
            this.headBox.TabStop = false;
            // 
            // eyeBox
            // 
            this.eyeBox.BackColor = System.Drawing.Color.Black;
            this.eyeBox.Location = new System.Drawing.Point(10, 808);
            this.eyeBox.Name = "eyeBox";
            this.eyeBox.Size = new System.Drawing.Size(535, 390);
            this.eyeBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.eyeBox.TabIndex = 21;
            this.eyeBox.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1121, 1224);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.autopilotLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.videoPlayer);
            this.Controls.Add(this.carBox);
            this.Controls.Add(this.headBox);
            this.Controls.Add(this.eyeBox);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "The Machine Learning Advantage Course";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.carBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.headBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.eyeBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label label3;
        private Accord.Controls.VideoSourcePlayer videoPlayer;
        private System.Windows.Forms.Label autopilotLabel;
        private System.Windows.Forms.PictureBox carBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox headBox;
        private System.Windows.Forms.PictureBox eyeBox;
    }
}

