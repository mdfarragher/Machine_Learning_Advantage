using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Accord.Imaging.Filters;
using Accord.Imaging;
using Accord;
using System.Collections.Generic;

namespace ml_csharp_lesson2
{
    public partial class MainForm : Form
    {
        // the last-detected lane lines
        private HoughLine[] laneLines = null;

        // the running frame counter
        private int frameCount = 0;

        /// <summary>
        /// Initialize MainForm.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set the video player to read a video file from disk.
        /// </summary>
        /// <param name="fileName"></param>
        private void SetVideo(string fileName)
        {
            var source = new Accord.Video.FFMPEG.VideoFileSource(fileName);
            videoPlayer.VideoSource = source;
        }

        /// <summary>
        /// Called when MainForm loads.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            SetVideo("./input.mp4");

            // start the player
            videoPlayer.Start();
        }

        /// <summary>
        /// Called when MainForm is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            videoPlayer.Stop();
        }

        /// <summary>
        /// Called when videoPlayer receives a new frame. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="image"></param>
        private void videoPlayer_NewFrame(object sender, ref Bitmap image)
        {
            // detect lane lines
            if (frameCount++ % 5 == 0)
                laneLines = DetectLaneLines(image);

            // draw the lanes on the main camera image
            if (laneLines != null)
            {
                Utility.DrawLaneLines(laneLines, image, Color.LightGreen, 2);

                // ... and in the bottom right box
                var laneImg = new Bitmap(image.Width, image.Height);
                Utility.DrawLaneLines(laneLines, laneImg, Color.White, 1);
                laneBox.Image = laneImg;
            }
        }

        /// <summary>
        /// Detect the highway lane boundaries.
        /// </summary>
        /// <param name="image">The camera frame to process</param>
        /// <returns>The detected lane lines in the frame</returns>
        private HoughLine[] DetectLaneLines(Bitmap image)
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************

            return new HoughLine[] { };  // replace this!
        }
    }
}
