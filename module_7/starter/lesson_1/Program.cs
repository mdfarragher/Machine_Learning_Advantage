using System;
using System.Collections.Generic;
using System.Linq;
using CNTK;
using Pensar;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace ml_csharp_lesson1
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
    /// The MyTrainingEngine class represents a custom training engine for this assignment.
    /// </summary>
    class MyTrainingEngine : TrainingEngine
    {
        /// <summary>
        /// Set up the feature variable.
        /// </summary>
        /// <returns>The feature variable to use.</returns>
        protected override Variable CreateFeatureVariable()
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************

            return null; // remove this when you're done!
        }

        /// <summary>
        /// Set up the label variable.
        /// </summary>
        /// <returns>The label variable to use.</returns>
        protected override Variable CreateLabelVariable()
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************

            return null; // remove this when you're done!
        }

        /// <summary>
        /// Create the model.
        /// </summary>
        /// <param name="features">The input feature to build the model on.</param>
        /// <returns>The completed model to use.</returns>
        protected override Function CreateModel(Variable features)
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************

            return null; // remove this when you're done!
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
            if (!System.IO.File.Exists("x_train_imdb.bin"))
            {
                Console.WriteLine("Unpacking archive...");
                DataUtil.Unzip(@"..\..\..\..\..\imdb_data.zip", ".");
            }

            // load training and test data
            Console.WriteLine("Loading data files...");
            var trainingPartitionX = DataUtil.LoadBinary<float>("x_train_imdb.bin", 25000, 500);
            var trainingPartitionY = DataUtil.LoadBinary<float>("y_train_imdb.bin", 25000);
            var testPartitionX = DataUtil.LoadBinary<float>("x_test_imdb.bin", 25000, 500);
            var testPartitionY = DataUtil.LoadBinary<float>("y_test_imdb.bin", 25000);

            // ******************
            // ADD YOUR CODE HERE
            // ******************

            Console.ReadLine();
        }
    }
}
