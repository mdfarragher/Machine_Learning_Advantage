using Microsoft.CognitiveServices.Speech;
using Microsoft.ProjectOxford.Search.Image;
using Microsoft.ProjectOxford.Vision;
using System;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ml_csharp_lesson4
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

        // ************************************
        // PUT YOUR SEARCH API KEY AND URL HERE
        // ************************************

        // Search API credentials
        private const string SEARCH_KEY = "...";
        private const string SEARCH_API = "...";

        // ***************************************
        // PUT YOUR SPEECH API KEY AND REGION HERE
        // ***************************************

        // Speech API credentials
        private const string SPEECH_KEY = "...";
        private const string SPEECH_REGION = "westus";

        /// <summary>
        /// The Speech API recognition client. 
        /// </summary>
        protected SpeechRecognizer speechRecognizer;

        /// <summary>
        /// The class constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Find the requested image and show it.
        /// </summary>
        /// <param name="searchString">The string to search for.</param>
        private async Task FindImage(string searchString)
        {
            // show the spinner
            Spinner.Visibility = Visibility.Visible;

            // find an image matching the query
            var img = await SearchForImage(searchString);
            if (img != null)
            {
                //// describe the image
                var description = await DescribeScene(img.ContentUrl);

                // show the image
                var image = new BitmapImage(new Uri(img.ContentUrl));
                var brush = new ImageBrush(image);
                brush.Stretch = Stretch.Uniform;
                brush.AlignmentX = AlignmentX.Left;
                brush.AlignmentY = AlignmentY.Top;
                MainCanvas.Background = brush;

                // hide the spinner
                Spinner.Visibility = Visibility.Hidden;

                // tell the canvas to redraw itself
                MainCanvas.InvalidateVisual();

                //// speak the description
                await SpeakDescription(description);
            }

            // hide the spinner
            else
                Spinner.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Perform a bing image search and return the top image.
        /// </summary>
        /// <param name="celebrity">The celebrity to search for.</param>
        /// <returns>The description of the celebrity.</returns>
        private async Task<Image> SearchForImage(string searchText)
        {
            var client = new ImageSearchClient(SEARCH_KEY);
            client.Url = SEARCH_API; // force use of v7 api

            var request = new ImageSearchRequest()
            {
                Query = searchText
            };
            var response = await client.GetImagesAsync(request);
            return response.Images.FirstOrDefault();
        }

        /// <summary>
        /// Detect what's in the given image.
        /// </summary>
        /// <param name="url">The url of the image to check.</param>
        /// <returns>An AnalysisResult instance that describes what's in the image.</returns>
        private async Task<string> DescribeScene(string url)
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************

            return null; // remove this when done!
        }

        /// <summary>
        /// Speak the image description.
        /// </summary>
        /// <param name="description">The image description to speak.</param>
        private async Task SpeakDescription(string description)
        {
            if (description != null)
            {
                // set up an adult female voice synthesizer
                var synth = new System.Speech.Synthesis.SpeechSynthesizer();
                synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);

                // speak the description using the builtin synthesizer
                await Task.Run(() => { synth.SpeakAsync(description); });
            }
        }

        /// <summary>
        /// Fires when the main window loads.
        /// </summary>
        /// <param name="sender">The main window.</param>
        /// <param name="e">The event arguments.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************
        }

        /// <summary>
        /// Fires when the search button is clicked.
        /// </summary>
        /// <param name="sender">The search button.</param>
        /// <param name="e">The event arguments</param>
        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************
        }

        /// <summary>
        /// Fires when the main window is unloaded.
        /// </summary>
        /// <param name="sender">The main window.</param>
        /// <param name="e">The event arguments.</param>
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            // dispose the speech recognition engine
            if (speechRecognizer != null)
                speechRecognizer.Dispose();
        }
    }
}
