using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Accord.Imaging.Filters;
using Accord.Imaging;
using Accord;

namespace ml_csharp_lesson1
{
    public partial class MainForm : Form
    {
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
            // set up a color filter for 'traffic sign yellow'
            var yellowFilter = new ColorFiltering(
                new IntRange(127, 255),
                new IntRange(127, 255),
                new IntRange(0, 127));

            // filter the frame so only yellow remains
            var yellowFrame = yellowFilter.Apply(bitmap);

            // convert the image to grayscale
            var grayFrame = Grayscale.CommonAlgorithms.BT709.Apply(yellowFrame);

            // use a sobel edge detector to find color edges
            var edgeDetector = new SobelEdgeDetector();
            var edgeFrame = edgeDetector.Apply(grayFrame);

            // threshold the edges
            var thresholdConverter = new Threshold(200);
            thresholdConverter.ApplyInPlace(edgeFrame);

            // show the thresholded image in the pip window
            edgeBox.Image = (Bitmap)edgeFrame.Clone();

            // use a blobcounter to find all shapes
            var detector = new BlobCounter()
            {
                FilterBlobs = true,
                CoupledSizeFiltering = true,
                MinWidth = 10,
                MinHeight = 10,
                MaxWidth = 200,
                MaxHeight = 150,
            };
            detector.ProcessImage(edgeFrame);
            var blobs = detector.GetObjectsInformation();

            // we're going to look for signs above the road and in the right curb,
            // so let's set up two search areas
            var searchArea1 = new Rectangle(
                (int)(edgeFrame.Width * 0.3),
                (int)(edgeFrame.Height * 0.1),
                (int)(edgeFrame.Width * 0.5),
                (int)(edgeFrame.Height * 0.54));
            var searchArea2 = new Rectangle(
                (int)(edgeFrame.Width * 0.6),
                (int)(edgeFrame.Height * 0.5),
                (int)(edgeFrame.Width * 0.4),
                (int)(edgeFrame.Height * 0.14));

            // let's draw the search areas into the main window 
            // for easy reference
            DrawRectangles(new[] { searchArea1, searchArea2 }, bitmap, Color.Red);

            // look for signs in both search areas
            var candidates =
                from shape in blobs
                where searchArea1.Contains(shape.Rectangle)
                  || searchArea2.Contains(shape.Rectangle)
                select shape;

            // build a list of all matches
            return (from shape in candidates
                    select shape.Rectangle).ToArray();
        }
    }
}
