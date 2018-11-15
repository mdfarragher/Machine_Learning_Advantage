using System;
using System.Collections.Generic;
using System.Linq;
using CNTK;
using Pensar;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace ml_csharp_lesson3
{
    /// <summary>
    /// The Plot class encapsulates a plotting window. 
    /// </summary>
    class Plot : System.Windows.Window
    {
        /// <summary>
        /// Construct a new instance of the class.
        /// </summary>
        /// <param name="results">The data to plot.</param>
        public Plot(List<List<double>> results)
        {
            // set up a new plot
            var plotModel = new PlotModel();
            plotModel.Title = "Training and Validation error";

            // add x and y axis
            plotModel.Axes.Add(
                new LinearAxis()
                {
                    Position = AxisPosition.Left,
                    Title = "Classification Error"
                }
            );
            plotModel.Axes.Add(
                new LinearAxis()
                {
                    Position = AxisPosition.Bottom,
                    Title = "Epochs"
                }
            );

            // add data series
            var labels = new string[] { "Training", "Validation" };
            var colors = new OxyColor[] { OxyColors.Blue, OxyColors.Green };
            for (int row = 0; row < results.Count; row++)
            {
                var lineSeries = new LineSeries
                {
                    ItemsSource = results[row].Select((value, index) => new DataPoint(index, value)),
                    Title = labels[row],
                    Color = colors[row]
                };
                plotModel.Series.Add(lineSeries);
            }

            // wrap up
            Title = "Chart";
            Content = new OxyPlot.Wpf.PlotView
            {
                Model = plotModel
            };
        }
    }

    /// <summary>
    /// The main application class.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main entry point. 
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        [STAThread]
        static void Main(string[] args)
        {
            // unpack archive
            Console.WriteLine("Loading data...");
            if (!System.IO.File.Exists("x_train.bin"))
            {
                DataUtil.Unzip(@"..\..\..\..\..\imdb_data.zip", ".");
            }

            // load training and test data
            var training_data = DataUtil.LoadBinary<float>("x_train.bin", 25000, 10000);
            var training_labels = DataUtil.LoadBinary<float>("y_train.bin", 25000);
            var test_data = DataUtil.LoadBinary<float>("x_test.bin", 25000, 10000);
            var test_labels = DataUtil.LoadBinary<float>("y_test.bin", 25000);

            // create feature and label variables
            var features = NetUtil.Var(new[] { 10000 }, DataType.Float);
            var labels = NetUtil.Var(new[] { 1 }, DataType.Float);

            // create neural network
            var network = features
                .Dense(16, CNTKLib.ReLU)
                .Dense(16, CNTKLib.ReLU)
                .Dense(1, CNTKLib.Sigmoid)
                .ToNetwork();

            // create loss and test functions
            var lossFunc = CNTKLib.BinaryCrossEntropy(network.Output, labels);
            var accuracyFunc = NetUtil.BinaryAccuracy(network.Output, labels);

            // use the Adam learning algorithm
            var learner = network.GetAdamLearner(
                learningRateSchedule: (0.001, 1),
                momentumSchedule: (0.9, 1),
                unitGain: true);

            // get a trainer for training, and an evaluator for testing the network
            var trainer = network.GetTrainer(learner, lossFunc, accuracyFunc);
            var evaluator = network.GetEvaluator(accuracyFunc);

            // declare some variables
            var trainingError = new List<double>();
            var validationError = new List<double>();
            var maxEpochs = 7;
            var batchSize = 32;
            var batchCount = 0;
            var error = 0.0;

            // train for a number of epochs
            Console.WriteLine("Training network...");
            for (int epoch = 0; epoch < maxEpochs; epoch++)
            {
                error = 0.0;
                batchCount = 0;

                // train the network using batches
                training_data.Index().Shuffle().Batch(
                    batchSize,
                    (indices, begin, end) =>
                    {
                        // get the current batch for training
                        var featureBatch = features.GetBatch(training_data, indices, begin, end);
                        var labelBatch = labels.GetBatch(training_labels, indices, begin, end);

                        // train the network on the batch
                        var result = trainer.TrainBatch(
                            new[]
                            {
                                (features, featureBatch),
                                (labels, labelBatch)
                            },
                            true
                        );
                        error += 1 - result.Evaluation;
                        batchCount++;
                    });
                trainingError.Add(error / batchCount);

                error = 0.0;
                batchCount = 0;

                // test the network using batches 
                test_data.Batch(
                    batchSize,
                    (data, begin, end) =>
                    {
                        // get current batch for testing
                        var featureBatch = features.GetBatch(test_data, begin, end);
                        var labelBatch = labels.GetBatch(test_labels, begin, end);

                        // test the network on the batch
                        error += 1 - evaluator.TestBatch(
                            new[]
                            {
                                (features, featureBatch),
                                (labels, labelBatch)
                            });

                        batchCount++;
                    });
                validationError.Add(error / batchCount);

                // show results for this epoch
                Console.Write($"Epoch {epoch + 1}/{maxEpochs}: ");
                Console.Write($"training error: {trainingError[epoch]}, ");
                Console.WriteLine($"validation error: {validationError[epoch]}");
            }

            // plot the training and validation curves
            var lines = new List<List<double>>() { trainingError, validationError };
            var app = new System.Windows.Application();
            app.Run(new Plot(lines));

            Console.ReadLine();
        }
    }
}
