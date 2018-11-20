using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CNTK;
using Pensar;

namespace ml_csharp_lesson3
{
    /// <summary>
    /// The plot window class for showing to show the training curve.
    /// </summary>
    class PlotWindow : System.Windows.Window
    {
        /// <summary>
        /// Construct a new plot.
        /// </summary>
        /// <param name="results">The data to plot.</param>
        public PlotWindow(List<List<double>> results)
        {
            var plotModel = new OxyPlot.PlotModel();
            plotModel.Title = "Cats And Dogs";
            plotModel.LegendPosition = OxyPlot.LegendPosition.BottomRight;

            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = "Accuracy",
                Minimum = 0,
                Maximum = 1,
            });
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Minimum = 0,
                Maximum = results[0].Count + 1,
                Title = "Epochs"
            });

            var labels = new string[] { "Train Set", "Validation Set" };
            var markerTypes = new OxyPlot.MarkerType[] { OxyPlot.MarkerType.Plus, OxyPlot.MarkerType.Circle };
            for (int row = 0; row < results.Count; row++)
            {
                var scatterSeries = new OxyPlot.Series.ScatterSeries()
                {
                    MarkerType = markerTypes[row],
                    MarkerStroke = OxyPlot.OxyColors.Blue,
                    MarkerFill = OxyPlot.OxyColors.Black
                };
                scatterSeries.ItemsSource = results[row].Select((value, index) => new OxyPlot.Series.ScatterPoint(index + 1, value));
                scatterSeries.Title = labels[row];
                plotModel.Series.Add(scatterSeries);
            }

            var plotView = new OxyPlot.Wpf.PlotView();
            plotView.Model = plotModel;

            Title = "Chart";
            Content = plotView;

        }
    }

    /// <summary>
    /// The main application class.
    /// </summary>
    class Program
    {
        // some training constants
        const int trainingSetSize = 1000;
        const int validationSetSize = 500;
        const int testSetSize = 500;
        const int imageWidth = 150;
        const int imageHeight = 150;
        const int numChannels = 3;
        const int maxEpochs = 10;
        const int batchSize = 32;

        /// <summary>
        /// Create the mapping files for features and labels
        /// </summary>
        static void CreateMapFiles()
        {
            var filenames = new string[] { "train_map.txt", "validation_map.txt", "test_map.txt" };
            var num_entries = new int[] { trainingSetSize, validationSetSize, testSetSize };
            var counter = 0;
            for (int j = 0; j < filenames.Length; j++)
            {
                var filePath = filenames[j];
                using (var dstFile = new StreamWriter(filePath))
                {
                    for (int i = 0; i < num_entries[j]; i++)
                    {
                        var catPath = Path.Combine("cat", $"{counter}.jpg");
                        var dogPath = Path.Combine("dog", $"{counter}.jpg");
                        dstFile.WriteLine($"{catPath}\t0");
                        dstFile.WriteLine($"{dogPath}\t1");
                        counter++;
                    }
                    Console.WriteLine($"  Created file: {filePath}");
                }
            }
        }

        /// <summary>
        /// The main entry point of the application.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        [STAThread]
        static void Main(string[] args)
        {
            // unzip archive
            if (!Directory.Exists("cat"))
            {
                Console.WriteLine("Unpacking data....");
                DataUtil.Unzip(@"..\..\..\..\..\catsanddogs.zip", ".");
            }

            // create mapping files
            if (!File.Exists("train_map.txt"))
            {
                Console.WriteLine("Creating mapping files...");
                CreateMapFiles();
            }

            // get a training and validation image reader
            var trainingReader = DataUtil.GetImageReader("train_map.txt", imageWidth, imageHeight, numChannels, 2, randomizeData: true, augmentData: true);
            var validationReader = DataUtil.GetImageReader("validation_map.txt", imageWidth, imageHeight, numChannels, 2, randomizeData: false, augmentData: false);

            // build features and labels
            var features = NetUtil.Var(new int[] { imageHeight, imageWidth, numChannels }, DataType.Float);
            var labels = NetUtil.Var(new int[] { 2 }, DataType.Float);

            // ******************
            // ADD YOUR CODE HERE
            // ******************

            // print the network to the console
            Console.WriteLine("Neural Network architecture: ");
            Console.WriteLine(network.ToSummary());

            // set up the loss function and the classification error function
            var lossFunc = CNTKLib.CrossEntropyWithSoftmax(network.Output, labels);
            var errorFunc = CNTKLib.ClassificationError(network.Output, labels);

            // use the Adam learning algorithm
            var learner = network.GetAdamLearner(
                learningRateSchedule: (0.0001, 1),
                momentumSchedule: (0.99, 1));

            // set up a trainer and an evaluator
            var trainer = network.GetTrainer(learner, lossFunc, errorFunc);
            var evaluator = network.GetEvaluator(errorFunc);

            // declare some variables
            var result = 0.0;
            var sampleCount = 0;
            var batchCount = 0;
            var lines = new List<List<double>>() { new List<double>(), new List<double>() };

            // train the network during several epochs
            Console.WriteLine("Training the neural network....");
            for (int epoch = 0; epoch < maxEpochs; epoch++)
            {
                Console.Write($"[{DateTime.Now:HH:mm:ss}] Training epoch {epoch+1}/{maxEpochs}... ");

                // train the network using random batches
                result = 0.0;
                sampleCount = 0;
                batchCount = 0;
                while (sampleCount < 2 * trainingSetSize)
                {
                    // get the current batch
                    var batch = trainingReader.GetBatch(batchSize);
                    var featuresBatch = batch[trainingReader.StreamInfo("features")];
                    var labelsBatch = batch[trainingReader.StreamInfo("labels")];

                    // train the network on the batch
                    var (Loss, Evaluation) = trainer.TrainBatch(
                        new[] {
                            (features, featuresBatch),
                            (labels,  labelsBatch)
                        }
                    );
                    result += Evaluation;
                    sampleCount += (int)featuresBatch.numberOfSamples;
                    batchCount++;
                }

                // show and store results
                var accuracy = 1 - result / batchCount;
                lines[0].Add(accuracy);
                Console.Write($"training accuracy: {accuracy}, ");

                // test the network using batches
                result = 0.0;
                sampleCount = 0;
                batchCount = 0;
                while (sampleCount < 2 * validationSetSize)
                {
                    // get the current batch for testing 
                    var batch = validationReader.GetBatch(batchSize);
                    var featuresBatch = batch[validationReader.StreamInfo("features")];
                    var labelsBatch = batch[validationReader.StreamInfo("labels")];

                    // test the network on the batch
                    result += evaluator.TestBatch(
                        new[] {
                            (features, featuresBatch),
                            (labels,  labelsBatch)
                        }
                    );
                    sampleCount += (int)featuresBatch.numberOfSamples;
                    batchCount++;
                }

                // show results
                accuracy = 1 - result / batchCount;
                lines[1].Add(accuracy);
                Console.WriteLine($"validation accuracy: {accuracy}");
            }

            // plot the training and validation curves
            var wpfApp = new System.Windows.Application();
            wpfApp.Run(new PlotWindow(lines));

            Console.ReadLine();
        }
    }
}
