using System;
using System.Collections.Generic;
using System.Linq;
using CNTK;
using Pensar;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace ml_csharp_lesson2
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
    /// The MyTrainingEngine class represents the training engine to use in this assignment.
    /// </summary>
    class MyTrainingEngine: TrainingEngine
    {
        /// <summary>
        /// Create the feature variable.
        /// </summary>
        /// <returns>The feature variable to use.</returns>
        protected override CNTK.Variable CreateFeatureVariable()
        {
            return NetUtil.Var(new int[] { 1 }, CNTK.DataType.Float);
        }

        /// <summary>
        /// Create the label variable.
        /// </summary>
        /// <returns>The label variable to use.</returns>
        protected override CNTK.Variable CreateLabelVariable()
        {
            var axis = new List<CNTK.Axis>() { CNTK.Axis.DefaultBatchAxis() };
            return NetUtil.Var(new int[] { 1 }, CNTK.DataType.Float, dynamicAxes: axis);
        }

        /// <summary>
        /// Create the model.
        /// </summary>
        /// <param name="features">The input feature to build the model on.</param>
        /// <returns>The completed model to use.</returns>
        protected override CNTK.Function CreateModel(CNTK.Variable features)
        {
            int numberOfClasses = 10000;
            int embeddingDimension = 32;
            int lstmUnits = 32;

            return features
                .OneHotOp(numberOfClasses, true)
                .Embedding(embeddingDimension)
                .LSTM(lstmUnits, lstmUnits)
                .Dense(1, CNTKLib.Sigmoid)
                .ToNetwork();
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
            Console.WriteLine("Unpacking archive...");
            if (!System.IO.File.Exists("x_train_imdb.bin"))
            {
                DataUtil.Unzip(@"..\..\..\..\..\imdb_data.zip", ".");
            }

            // load training and test data
            Console.WriteLine("Loading data files...");
            var trainingPartitionX = DataUtil.LoadBinary<float>("x_train_imdb.bin", 25000, 500);
            var trainingPartitionY = DataUtil.LoadBinary<float>("y_train_imdb.bin", 25000);
            var testPartitionX = DataUtil.LoadBinary<float>("x_test_imdb.bin", 25000, 500);
            var testPartitionY = DataUtil.LoadBinary<float>("y_test_imdb.bin", 25000);

            // reserve the final 20% of training data for validation
            var pivot = (int)(trainingPartitionX.Length * 0.8);
            var trainingFeatures = trainingPartitionX.Skip(pivot).ToArray();
            var trainingLabels = trainingPartitionY.Skip(pivot).ToArray();
            var validationFeatures = trainingPartitionX.Take(pivot).ToArray();
            var validationLabels = trainingPartitionY.Take(pivot).ToArray();

            // set up a new training engine
            Console.WriteLine("Seting up training engine...");
            var engine = new MyTrainingEngine()
            {
                NumberOfEpochs = 10,
                BatchSize = 128,
                SequenceLength = 500
            };

            // load the data into the engine
            engine.SetData(trainingFeatures, trainingLabels, validationFeatures, validationLabels);

            // start the training
            Console.WriteLine("Start training...");
            engine.Train();

            // plot the training and validation curves
            var app = new System.Windows.Application();
            app.Run(new Plot(engine.TrainingCurves));

            Console.ReadLine();
        }
    }
}
