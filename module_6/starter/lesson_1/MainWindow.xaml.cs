using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ml_csharp_lesson1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Face API credentials
        // **********************************
        // PUT YOUR FACE API KEY AND URL HERE
        // **********************************
        private const string FACE_KEY = "...";
        private const string FACE_API = "...";

        /// <summary>
        /// The celebrity image to analyze. 
        /// </summary>
        protected BitmapImage image;

        /// <summary>
        /// The array of detected faces.
        /// </summary>
        protected Face[] faces;

        /// <summary>
        /// The class constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Run a complete face detection on a new image.
        /// </summary>
        private async Task RunFaceDetection()
        {
            // show the spinner
            Spinner.Visibility = Visibility.Visible;

            // clear any existing face rectangles
            var toDelete = new List<UIElement>(MainCanvas.Children.OfType<System.Windows.Shapes.Rectangle>());
            foreach (var element in toDelete)
                MainCanvas.Children.Remove(element);

            // detect all faces in image
            faces = await DetectFaces(image);

            // draw face rectangles on the canvas
            foreach (var face in faces)
            {
                DrawFaceRectangle(face);
            }

            // hide the spinner
            Spinner.Visibility = Visibility.Hidden;

            // tell the canvas to redraw itself
            MainCanvas.InvalidateVisual();
        }

        /// <summary>
        /// Detect all faces in a given image.
        /// </summary>
        /// <param name="image">The image to check for faces.</param>
        /// <returns>An array of Face instances describing each face in the image.</returns>
        private async Task<Face[]> DetectFaces(BitmapImage image)
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************

            return new Face[] { }; // replace this when done!
        }

        /// <summary>
        /// Draw the rectangle of the given face.
        /// </summary>
        /// <param name="face">The face to draw the rectangle for.</param>
        private void DrawFaceRectangle(Face face)
        {
            // calculate scaling factor - ONLY WORKS FOR LANDSCAPE IMAGES
            var scaleX = MainCanvas.ActualWidth / image.PixelWidth;
            var scaleY = 1; // MainCanvas.ActualHeight / image.PixelHeight;

            // *************************
            // CHANGE THE FOLLOWING CODE
            // *************************

            // create face rectangle
            var rectangle = new System.Windows.Shapes.Rectangle
            {
                Stroke = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                Fill = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255)),
                StrokeThickness = 2,
                Width = face.FaceRectangle.Width * scaleX,
                Height = face.FaceRectangle.Height * scaleY,
                Tag = face.FaceId
            };
            Canvas.SetLeft(rectangle, face.FaceRectangle.Left * scaleX);
            Canvas.SetTop(rectangle, face.FaceRectangle.Top * scaleY);

            // add hover effect
            rectangle.MouseEnter += (sender, e) => {
                ((System.Windows.Shapes.Rectangle)sender).StrokeThickness = 5; };
            rectangle.MouseLeave += (sender, e) => {
                ((System.Windows.Shapes.Rectangle)sender).StrokeThickness = 2; };

            // add click handler
            rectangle.MouseLeftButtonDown += Rectangle_MouseLeftButtonDown;

            MainCanvas.Children.Add(rectangle);
        }

        /// <summary>
        /// Fires when the main window has initialized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Initialized(object sender, EventArgs e)
        {
            // load the image
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "EllenSelfie.jpg");
            image = new BitmapImage(new Uri(path));
            var brush = new ImageBrush(image);
            brush.Stretch = Stretch.UniformToFill;
            brush.AlignmentX = AlignmentX.Left;
            brush.AlignmentY = AlignmentY.Top;
            MainCanvas.Background = brush;
        }

        /// <summary>
        /// Fires when the main window has loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await RunFaceDetection();
        }

        /// <summary>
        /// Handle a left mouse button down event on a face rectangle.
        /// </summary>
        /// <param name="sender">the face rectangle that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void Rectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************
        }

        /// <summary>
        /// Called when the OpenButton is clicked.
        /// </summary>
        /// <param name="sender">the button that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            // open file dialog
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                image = new BitmapImage(new Uri(openFileDialog.FileName));

                // set the new background image
                var brush = new ImageBrush(image);
                brush.Stretch = Stretch.UniformToFill;
                brush.AlignmentX = AlignmentX.Left;
                brush.AlignmentY = AlignmentY.Top;
                MainCanvas.Background = brush;

                // run face detection on new image
                await RunFaceDetection();

            }
        }
    }
}
