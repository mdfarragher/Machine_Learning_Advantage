using Microsoft.Azure.CognitiveServices.Search.EntitySearch;
using Microsoft.Azure.CognitiveServices.Search.EntitySearch.Models;
using Microsoft.ProjectOxford.Vision;
using Microsoft.Win32;
using Newtonsoft.Json;
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

namespace ml_csharp_lesson2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // ************************************
        // PUT YOUR VISION API KEY AND URL HERE
        // ************************************

        // Vision API credentials
        private const string VISION_KEY = "...";
        private const string VISION_API = "...";

        // ************************************************
        // PUT YOUR BING ENTITY SEARCH API KEY AND URL HERE
        // ************************************************

        // Bing Entity Search API credentials
        private const string ENTITY_KEY = "...";
        private const string ENTITY_API = "...";

        /// <summary>
        /// The celebrity image to analyze. 
        /// </summary>
        protected BitmapImage image;

        /// <summary>
        /// The current list of celebrities
        /// </summary>
        protected Celebrity[] celebrities;

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

            // clear any existing face rectangles and labels
            var toDelete = new List<UIElement>();
            foreach (var element in MainCanvas.Children.OfType<UIElement>())
            {
                if (element is System.Windows.Shapes.Rectangle || element is Label)
                    toDelete.Add(element);
            }
            foreach (var element in toDelete)
                MainCanvas.Children.Remove(element);

            // detect all faces in image
            celebrities = await DetectCelebrities(image);

            // draw face rectangles on the canvas
            foreach (var celebrity in celebrities)
            {
                DrawFaceRectangle(celebrity);
            }

            // hide the spinner
            Spinner.Visibility = Visibility.Hidden;

            // tell the canvas to redraw itself
            MainCanvas.InvalidateVisual();
        }

        /// <summary>
        /// Detect all celebrities in a given image.
        /// </summary>
        /// <param name="image">The image to check for celebrities.</param>
        /// <returns>An AnalysisResult instance that describes each celebrity in the image.</returns>
        private async Task<Celebrity[]> DetectCelebrities(BitmapImage image)
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************

            return new Celebrity[] { }; // remove this when you're done!
        }

        /// <summary>
        /// Draw the rectangle of the given celebrity.
        /// </summary>
        /// <param name="celebrity">The celebrity to draw the rectangle for.</param>
        private void DrawFaceRectangle(Celebrity celebrity)
        {
            // calculate scaling factor - ONLY WORKS FOR LANDSCAPE IMAGES
            var scaleX = MainCanvas.ActualWidth / image.PixelWidth;
            var scaleY = MainCanvas.ActualHeight / image.PixelHeight;

            // create face rectangle
            var rectangle = new System.Windows.Shapes.Rectangle
            {
                Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 0)),
                Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0, 255, 255, 255)),
                StrokeThickness = 2,
                Width = celebrity.FaceRectangle.Width * scaleX,
                Height = celebrity.FaceRectangle.Height * scaleY,
                Tag = celebrity.Name
            };
            Canvas.SetLeft(rectangle, celebrity.FaceRectangle.Left * scaleX);
            Canvas.SetTop(rectangle, celebrity.FaceRectangle.Top * scaleY);

            // add handlers
            rectangle.MouseEnter += (sender, e) => {
                ((System.Windows.Shapes.Rectangle)sender).Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0)); };
            rectangle.MouseLeave += (sender, e) => {
                ((System.Windows.Shapes.Rectangle)sender).Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 0)); };
            rectangle.MouseLeftButtonDown += Rectangle_MouseLeftButtonDown;

            MainCanvas.Children.Add(rectangle);

            // ******************
            // ADD YOUR CODE HERE
            // ******************

        }

        /// <summary>
        /// Perform an entity search on the celebrity and return the description.
        /// </summary>
        /// <param name="celebrity">The celebrity to search for.</param>
        /// <returns>The description of the celebrity.</returns>
        private async Task<Thing> EntitySearch(Celebrity celebrity)
        {
            var client = new EntitySearchAPI(new ApiKeyServiceClientCredentials(ENTITY_KEY));
            var data = await client.Entities.SearchAsync(query: celebrity.Name);
            if (data?.Entities?.Value?.Count > 0)
            {
                return (from v in data.Entities.Value
                        where v.EntityPresentationInfo.EntityScenario == EntityScenario.DominantEntity
                        select v).FirstOrDefault();
            }
            else
                return null;
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
            brush.Stretch = Stretch.Uniform;
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
        private async void Rectangle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // start the secondary spinner
            Spinner2.Visibility = Visibility.Visible;

            // remove any existing image
            FaceImage.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));

            // ******************
            // ADD YOUR CODE HERE
            // ******************

            // stop the secondary spinner
            Spinner2.Visibility = Visibility.Hidden;
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
                brush.Stretch = Stretch.Uniform;
                brush.AlignmentX = AlignmentX.Left;
                brush.AlignmentY = AlignmentY.Top;
                MainCanvas.Background = brush;

                // run face detection on new image
                await RunFaceDetection();

            }
        }
    }
}
