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
        private const string FACE_KEY = "a2be78d38ff541b397168a80b2b7d597";
        private const string FACE_API = "https://westeurope.api.cognitive.microsoft.com/face/v1.0";

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
            // write the image to a stream
            var stream = new MemoryStream();
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(stream);
            stream.Seek(0, SeekOrigin.Begin);

            // detect faces    
            var faceClient = new FaceServiceClient(FACE_KEY, FACE_API);
            var attributes = new FaceAttributeType[] { FaceAttributeType.Age, FaceAttributeType.Accessories, FaceAttributeType.Emotion, FaceAttributeType.FacialHair, FaceAttributeType.Gender, FaceAttributeType.Glasses, FaceAttributeType.Hair, FaceAttributeType.Makeup };
            return await faceClient.DetectAsync(stream, true, false, attributes);
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

            // draw men in blue and women in pink
            var color = face.FaceAttributes.Gender == "male" ?
              Color.FromRgb(0, 0, 255) : Color.FromRgb(255, 20, 147);

            // create face rectangle
            var rectangle = new System.Windows.Shapes.Rectangle
            {
                Stroke = new SolidColorBrush(color),
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
            // helper method to convert glasses type to string
            string GlassesToString(Glasses glasses)
            {
                switch(glasses)
                {
                    case Microsoft.ProjectOxford.Face.Contract.Glasses.NoGlasses:
                        return "none";
                    case Microsoft.ProjectOxford.Face.Contract.Glasses.ReadingGlasses:
                        return "reading glasses";
                    case Microsoft.ProjectOxford.Face.Contract.Glasses.Sunglasses:
                        return "sunglasses";
                    case Microsoft.ProjectOxford.Face.Contract.Glasses.SwimmingGoggles:
                        return "swimming goggles";
                    default:
                        return "unknown glasses";
                }
            }

            // helper method to convert accessory type to string
            string AccessoryToString(AccessoryType accessory)
            {
                switch (accessory)
                {
                    case AccessoryType.Glasses:
                        return "glasses";
                    case AccessoryType.Headwear:
                        return "headwear";
                    case AccessoryType.Mask:
                        return "mask";
                    default:
                        return "unknown accessory";
                }
            }

            // find the face instance describing this rectangle
            var rectangle = (System.Windows.Shapes.Rectangle)sender;
            var face = (from f in faces
                        where f.FaceId == (Guid)rectangle.Tag
                        select f).FirstOrDefault();

            // update labels
            if (face != null)
            {
                var attr = face.FaceAttributes;
                Gender.Content = $"Gender: {attr.Gender}";
                Age.Content = $"Age: {attr.Age}";
                Emotion.Content = $"Emotion: {attr.Emotion.ToRankedList().First().Key.ToLower()}";
                Hair.Content = $"Hair: {attr.Hair.HairColor.OrderByDescending(h => h.Confidence).FirstOrDefault()?.Color.ToString().ToLower()}";
                Beard.Content = $"Beard: {100 * attr.FacialHair.Beard}%";
                Moustache.Content = $"Moustache: {100 * attr.FacialHair.Moustache}%";
                Glasses.Content = $"Glasses: {GlassesToString(attr.Glasses)}";
                EyeMakeup.Content = $"Eye Makeup: {(attr.Makeup.EyeMakeup ? "yes" : "no")}";
                LipMakeup.Content = $"Lip Makeup: {(attr.Makeup.LipMakeup ? "yes" : "no")}";

                // show accessories
                var accessories = from a in attr.Accessories select AccessoryToString(a.Type);
                Accessories.Content = $"Accessories: {string.Join(",", accessories) }";

                // update the image
                var cropped = new CroppedBitmap(image, new Int32Rect(face.FaceRectangle.Left, face.FaceRectangle.Top, face.FaceRectangle.Width, face.FaceRectangle.Height));
                var brush = new ImageBrush(cropped);
                brush.Stretch = Stretch.Uniform;
                FaceImage.Background = brush;
            }
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
