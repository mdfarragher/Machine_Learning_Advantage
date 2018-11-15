using System;
using System.Collections.Generic;
using System.Linq;
using CNTK;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Pensar;

namespace ml_csharp_lesson4
{
    /// <summary>
    /// The Plot class encapsulates a plotting window. 
    /// </summary>
    class Plot : System.Windows.Window
    {
        /// <summary>
        /// Construct a new instance of the class.
        /// </summary>
        /// <param name="title">The plot title.</param>
        /// <param name="results">The data to plot.</param>
        public Plot(string title, List<List<double>> results)
        {
            // set up plot model
            var plotModel = new OxyPlot.PlotModel();
            plotModel.Title = title;

            // set up axes and colors
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis() { Position = OxyPlot.Axes.AxisPosition.Left, Title = "Error" });
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis() { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Epochs" });
            var colors = new OxyPlot.OxyColor[] { OxyPlot.OxyColors.Blue, OxyPlot.OxyColors.Green, OxyPlot.OxyColors.Red, OxyPlot.OxyColors.Black };

            // set up lines
            for (int i = 0; i < results.Count; i++)
            {
                var lineSeries = new OxyPlot.Series.LineSeries();
                lineSeries.ItemsSource = results[i].Select((value, index) => new OxyPlot.DataPoint(index, value));
                lineSeries.Title = string.Format("KFold {0}/{1}", i + 1, results.Count);
                //lineSeries.Color = colors[i];
                plotModel.Series.Add(lineSeries);
            }

            var plotView = new OxyPlot.Wpf.PlotView();
            plotView.Model = plotModel;

            Title = title;
            Content = plotView;
        }
    }

    /// <summary>
    /// The main application class.
    /// </summary>
    class Program
    {
        // local members
        private static CNTK.Variable features;
        private static CNTK.Variable labels;
        private static CNTK.Trainer trainer;
        private static CNTK.Evaluator evaluator;

        /// <summary>
        /// Create the neural network for this app.
        /// </summary>
        /// <returns>The neural network to use</returns>
        public static CNTK.Function CreateNetwork()
        {
            // build features and labels
            features = NetUtil.Var(new int[] { 13 }, DataType.Float);
            labels = NetUtil.Var(new int[] { 1 }, DataType.Float);

            // build the network
            var network = features
                .Dense(64, CNTKLib.ReLU)
                .Dense(64, CNTKLib.ReLU)
                .Dense(1)
                .ToNetwork();

            // set up the loss function and the classification error function
            var lossFunc = NetUtil.MeanSquaredError(network.Output, labels);
            var errorFunc = NetUtil.MeanAbsoluteError(network.Output, labels);

            // use the Adam learning algorithm
            var learner = network.GetAdamLearner(
                learningRateSchedule: (0.001, 1),
                momentumSchedule: (0.9, 1),
                unitGain: true);

            // set up a trainer and an evaluator
            trainer = network.GetTrainer(learner, lossFunc, errorFunc);
            evaluator = network.GetEvaluator(errorFunc);

            return network;
        }

        /// <summary>
        /// The main entry point of the application.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        [STAThread]
        public static void Main(string[] args)
        {
            Console.WriteLine("Loading data....");

            // unzip archive
            if (!System.IO.File.Exists("x_train.bin"))
            {
                DataUtil.Unzip(@"..\..\..\..\..\boston_housing.zip", ".");
            }

            // load training and test data
            var training_data = DataUtil.LoadBinary<float>("x_train.bin", 404, 13);
            var test_data = DataUtil.LoadBinary<float>("x_test.bin", 102, 13);
            var training_labels = DataUtil.LoadBinary<float>("y_train.bin", 404);
            var test_labels = DataUtil.LoadBinary<float>("y_test.bin", 102);

            // report results
            Console.WriteLine($"{training_data.GetLength(0)} training houses loaded");
            Console.WriteLine($"{test_data.GetLength(0)} test houses loaded");

            // declare some variables
            var numFolds = 4;
            var maxEpochs = 50;
            var batchSize = 16;
            var batchCount = 0;

            // partition the training data using KFolds
            var lines = new List<List<double>>();
            Console.WriteLine("Training the neural network....");
            training_data.Index().Shuffle().KFold(numFolds, (foldIndex, trainingIndices, validationIndices) =>
            {
                var line = new List<double>();
                Console.WriteLine($"KFold partition {foldIndex + 1}/{numFolds}");

                // create a new network
                var network = CreateNetwork();

                // train the network during several epochs
                for (int epoch = 0; epoch < maxEpochs; epoch++)
                {

                    // train the network using random batches
                    var trainingError = 0.0;
                    batchCount = 0;
                    trainingIndices.Batch(batchSize, (indices, begin, end) =>
                    {
                        // get the current batch
                        var featureBatch = features.GetBatch(training_data, indices, begin, end);
                        var labelBatch = labels.GetBatch(training_labels, indices, begin, end);

                        // train the network on the batch
                        var result = trainer.TrainBatch(
                            new[] {
                                (features, featureBatch),
                                (labels,  labelBatch)
                            },
                            false
                        );
                        trainingError += result.Evaluation;
                        batchCount++;
                    });
                    trainingError /= batchCount;

                    // test the network using batches
                    var validationError = 0.0;
                    batchCount = 0;
                    validationIndices.Batch(batchSize, (data, begin, end) =>
                    {
                        // get the current batch for testing 
                        var featureBatch = features.GetBatch(test_data, begin, end);
                        var labelBatch = labels.GetBatch(test_labels, begin, end);

                        // test the network on the batch
                        validationError += evaluator.TestBatch(
                            new[] {
                                (features, featureBatch),
                                (labels,  labelBatch)
                            }
                        );
                        batchCount++;
                    });
                    validationError /= batchCount;

                    // show results
                    if (epoch % 10 == 9)
                        Console.WriteLine($"Epoch {epoch + 1}/{maxEpochs}... training error: {trainingError}, validation error: {validationError}");

                    // add validation error to list
                    line.Add(validationError);
                }
                lines.Add(line);
            });

            // calculate and plot the average error over all folds
            var averageError = (from i in Enumerable.Range(0, maxEpochs)
                                select (from j in Enumerable.Range(0, lines.Count())
                                        select lines[j][i]).Average()).ToList();

            // plot the results
            var app = new System.Windows.Application();
            app.Run(new Plot("Mean Absolute Validation Error Per KFold", lines));
            //lines.Clear();
            //lines.Add(averageError);
            //app.Run(new Plot("Mean Absolute Validation Error For All KFolds", lines));

            Console.ReadLine();
        }
    }
}
