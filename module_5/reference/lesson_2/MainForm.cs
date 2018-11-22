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
    /// <summary>
    /// A LineRecord struct holds one detected hough line
    /// </summary>
    public struct LineRecord
    {
        /// <summary>
        /// The frame index when the line was discovered
        /// </summary>
        public int StartFrameIndex;

        /// <summary>
        /// The theta angle bucket for this line
        /// </summary>
        public int ThetaBucket;

        /// <summary>
        /// The detected hough line
        /// </summary>
        public HoughLine Line; 
    }

    public partial class MainForm : Form
    {
        // a dictionary of last-detected lane lines
        private List<LineRecord> laneLines = new List<LineRecord>();

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
            {
                // store lines in buckets of 10 degrees
                var lines = DetectLaneLines(image);
                foreach (var line in lines)
                {
                    var bucket = (int)(line.Theta / 19);
                    if (!laneLines.Any(v => v.ThetaBucket == bucket))
                    {
                        var lineRecord = new LineRecord()
                        {
                            StartFrameIndex = frameCount,
                            ThetaBucket = bucket,
                            Line = line
                        };
                        laneLines.Add(lineRecord);
                    }
                }
            }

            // retire old lines 
            laneLines.RemoveAll(l => frameCount - l.StartFrameIndex > 100);

            // get the remaining lane lines
            var survivingLines = (from l in laneLines
                                  select l.Line).ToArray();

            // draw the lanes on the main camera image
            if (laneLines != null)
            {
                Utility.DrawLaneLines(survivingLines, image, Color.LightGreen, 2);

                // ... and in the bottom right box
                var laneImg = new Bitmap(image.Width, image.Height);
                Utility.DrawLaneLines(survivingLines, laneImg, Color.White, 1);
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
            // convert image to grayscale
            var frame = Grayscale.CommonAlgorithms.BT709.Apply(image);

            // threshold image to only keep light pixels
            var threshold = new Threshold(125);
            threshold.ApplyInPlace(frame);

            // blank out everything but the road
            var horizonY = (int)(image.Height * 0.65);
            var fill = new CanvasFill(new Rectangle(0, 0, image.Width, horizonY), Color.Red);
            fill.ApplyInPlace(frame);

            // detect edges
            var edgeDetector = new CannyEdgeDetector();
            edgeDetector.ApplyInPlace(frame);

            // do a hough line transformation, which will search for straight lines in the frame
            var transform = new HoughLineTransformation();
            transform.ProcessImage(frame);
            var rawLines = transform.GetMostIntensiveLines(50);

            // only keep non-horizontal lines that cross the horizon at the vanishing point
            var lines = from l in rawLines
                        let range = new Range(-75, -65)
                        where range.IsInside(l.Radius)
                            && (l.Theta <= 85 || l.Theta >= 95)
                        select l;

            // show the edge detection view in the bottom left box
            edgeBox.Image = (Bitmap)frame.Clone();

            // return lines
            return lines.ToArray();
        }
    }
}
