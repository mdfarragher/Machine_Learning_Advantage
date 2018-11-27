using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Accord.Imaging.Filters;
using Accord.Imaging;
using System.Threading;

namespace ml_csharp_lesson1
{
    public partial class MainForm : Form
    {
        private int _matchesCount;
        private readonly SynchronizationContext _synchronizationContext;

        /// <summary>
        /// Initialize MainForm.
        /// </summary>
        public MainForm()
        {
            _synchronizationContext = WindowsFormsSynchronizationContext.Current;
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
            SetVideo(@"D:\MachineLearningCourseMark\Machine_Learning_Advantage-master\Machine_Learning_Advantage\module_5\starter\input.mp4");
           // SetVideo("./input.mp4");
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
        private void VideoPlayer_NewFrame(object sender, ref Bitmap image)
        {
            // detect all traffic signs in the image
            var trafficSigns = FindTrafficSigns(image);

            // draw the rectangles in the bottom right picturebox
            var trafficSignsImg = new Bitmap(image.Width, image.Height);
            DrawRectangles(trafficSigns, trafficSignsImg, Color.White);
            trafficSignBox.Image = trafficSignsImg;

            // highlight each sign in the main image with a green rectangle
            DrawRectangles(trafficSigns, image, Color.LightGreen);
        }

        /// <summary>
        /// Draw rectangles in the specified image.
        /// </summary>
        /// <param name="rectangles">The array of rectangles to draw</param>
        /// <param name="image">The image to draw the rectangles in</param>
        /// <param name="color">The drawing color to use</param>
        private void DrawRectangles(Rectangle[] rectangles, Bitmap image, Color color)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                foreach (var rect in rectangles)
                {
                    var pen = new Pen(color)
                    {
                        Width = 4f
                    };
                    g.DrawLine(pen, rect.X, rect.Y, rect.X + rect.Width, rect.Y);
                    g.DrawLine(pen, rect.X, rect.Y, rect.X, rect.Y + rect.Height);
                    g.DrawLine(pen, rect.X + rect.Width, rect.Y, rect.X + rect.Width, rect.Y + rect.Height);
                    g.DrawLine(pen, rect.X, rect.Y + rect.Height, rect.X + rect.Width, rect.Y + rect.Height);
                }
            }
        }

        /// <summary>
        /// Find all traffic signs in the specified bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap in which to search for traffic signs</param>
        /// <returns>A Rectangle[] array of found traffic signs</returns>
        private Rectangle[] FindTrafficSigns(Bitmap bitmap)
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************

            var matches =  new Rectangle[] { };  // replace this!

            _matchesCount += matches.Length;

            _synchronizationContext.Send(d =>
            {
                _currentFrameMatchesTextBox.Text = matches.Length.ToString();
                _totalFrameMatchesTextBox.Text = _matchesCount.ToString();
            }, null);

            return matches;
        }
    }
}
