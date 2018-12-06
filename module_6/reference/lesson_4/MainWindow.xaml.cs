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
        // Vision API credentials
        private const string VISION_KEY = "6e546577124f45e2ab0b926c6559c434";
        private const string VISION_API = "https://westeurope.api.cognitive.microsoft.com/vision/v1.0";

        // Search API credentials
        private const string SEARCH_KEY = "a29c17f1264c47378f077c8a6cc5cf29";
        private const string SEARCH_API = "https://api.cognitive.microsoft.com/bing/v7.0/images/search";

        // Speech API credentials
        private const string SPEECH_KEY = "95e7fb94f64743169b08630e87649792";
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
            // analyze image and get list of possible captions
            var visionClient = new VisionServiceClient(VISION_KEY, VISION_API);
            var result = await visionClient.DescribeAsync(url);

            // get the caption with the highest confidence
            var caption = (from c in result.Description.Captions
                           orderby c.Confidence descending
                           select c.Text).FirstOrDefault();

            // pick a random response
            string[] responses = new string[]
            {
                "Okay Mark, I found a picture of {0}",
                "Here's a picture of {0}",
                "I found an image of {0}",
                "Check out this picture I found of {0}",
                "Mark, how about this: a picture of {0}"
            };
            var rnd = new Random();
            var response = responses[rnd.Next(responses.Length)];

            return string.Format(response, caption ?? "something");
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
            // create a speech API client configuration
            var config = SpeechConfig.FromSubscription(SPEECH_KEY, SPEECH_REGION);

            // create an english-language speech recognizer
            speechRecognizer = new SpeechRecognizer(config);
        }

        /// <summary>
        /// Fires when the search button is clicked.
        /// </summary>
        /// <param name="sender">The search button.</param>
        /// <param name="e">The event arguments</param>
        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // show that the app is listening
            QueryText.Text = "<listening>";

            // listen for a single command
            var result = await speechRecognizer.RecognizeOnceAsync();

            // handle recognized speech
            if (result.Reason == ResultReason.RecognizedSpeech)
            {
                // find the image
                await FindImage(result.Text);

                // show the spoken command in the text box
                QueryText.Text = result.Text;
            }

            // handle unrecognized speech
            else if (result.Reason == ResultReason.NoMatch)
                QueryText.Text = "??? I'm sorry, I didn't understand that ???";

            // handle cancelled recognition
            else if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = CancellationDetails.FromResult(result);
                QueryText.Text = $"RECOGNITION CANCELED: {cancellation.Reason}";
            }

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
