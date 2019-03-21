using CNTK;
using Microsoft.Win32;
using Cv = OpenCvSharp;
using Pensar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ml_csharp_lesson2b
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The path of the image to analyze. 
        /// </summary>
        protected string imagePath;

        /// <summary>
        /// The current image description.
        /// </summary>
        protected string description;

        /// <summary>
        /// The neural network to use;
        /// </summary>
        protected Function network = null;

        /// <summary>
        /// The class constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Run a complete detection on a new image.
        /// </summary>
        private void RunDetection()
        {
            // clear the label
            Description.Text = "---";

            // show the spinner
            Spinner.Visibility = Visibility.Visible;

            // detect what's in the image
            (var cat, var dog) = DetectScene(imagePath);

            // build the description
            var description = cat > dog ? "Cat" : "Dog";
            description += $" detected - cat confidence: {cat * 100:0}%";
            description += $", dog confidence: {dog * 100:0}%";

            // show the description
            Description.Text = description;

            // hide the spinner
            Spinner.Visibility = Visibility.Hidden;

            // tell the canvas to redraw itself
            MainCanvas.InvalidateVisual();
        }

        /// <summary>
        /// Detect what's in the given image.
        /// </summary>
        /// <param name="imagePath">The path of the image to check.</param>
        /// <returns>A tuple of probabilities indicating if the image contains a dog or a cat.</returns>
        private (double cat, double dog) DetectScene(string imagePath)
        {
            // load the neural network
            if (network == null)
            {
                var path = @"..\..\..\..\..\models\catdogdetector.model";
                network = Function.Load(path, NetUtil.CurrentDevice);
            }

            // grab the image
            var imageData = new float[150 * 150 * 3];
            using (var mat = Cv.Cv2.ImRead(imagePath))
            {
                using (var mat2 = new Cv.Mat(150, 150, mat.Type()))
                {
                    Cv.Cv2.Resize(mat, mat2, new OpenCvSharp.Size(150, 150));
                    imageData = StyleTransfer.FlattenByChannel(mat2, new float[] { 0, 0, 0 });
                }
            }

            // set up a tensor to hold the image data
            var imageValue = new CNTK.NDArrayView(
                new int[] { 150, 150, 3 }, 
                imageData, 
                NetUtil.CurrentDevice);

            // set up input and output dictionaries
            var inputs = new Dictionary<CNTK.Variable, CNTK.Value>()
            {
                { network.Arguments[0], new CNTK.Value(imageValue) }
            };
            var outputs = new Dictionary<CNTK.Variable, CNTK.Value>()
            {
                { network.Outputs[0], null }
            };

            // run the neural network
            network.Evaluate(inputs, outputs, NetUtil.CurrentDevice);

            // return the result
            var key = network.Outputs[0];
            var result = outputs[key].GetDenseData<float>(key);
            return (result[0][0], result[0][1]); 
        }

        /// <summary>
        /// Load a random dog or cat image.
        /// </summary>
        private void LoadRandomAnimal()
        {
            // select a random animal
            var rnd = new Random();
            var num = rnd.Next(2000);
            var animal = rnd.Next(2) == 0 ? "dog" : "cat";
            imagePath = Path.GetFullPath($@"..\..\..\..\lesson_2\bin\x64\Release\{animal}\{num}.jpg");

            // load the image
            var image = new BitmapImage(new Uri(imagePath));
            var brush = new ImageBrush(image)
            {
                Stretch = Stretch.Uniform,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top
            };
            MainCanvas.Background = brush;
        }

        /// <summary>
        /// Fires when the main window has initialized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Initialized(object sender, EventArgs e)
        {
            LoadRandomAnimal();
        }

        /// <summary>
        /// Fires when the main window has loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RunDetection();
        }


        /// <summary>
        /// Called when the OpenButton is clicked.
        /// </summary>
        /// <param name="sender">the button that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            // load a new animal and run detection
            LoadRandomAnimal();
            RunDetection();
        }
    }
}
