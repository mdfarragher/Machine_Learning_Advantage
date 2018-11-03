using Accord.Controls;
using Accord.Math;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Models.Regression.Linear;
using Accord.Statistics.Visualizations;
using Deedle;
using System;
using System.IO;
using System.Linq;

namespace ml_csharp_lesson1
{
    /// <summary>
    /// The main application class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Plot a graph on screen.
        /// </summary>
        /// <param name="feature">The features to plot</param>
        /// <param name="labels">The labels to plot</param>
        /// <param name="predictions">The predictions to plot</param>
        /// <param name="title">The plot title</param>
        /// <param name="xAxisLabel">The x-axis label</param>
        /// <param name="yAxisLabel">The y-axis label</param>
        public static void Plot(
            double[] feature,
            double[] labels,
            double[] predictions,
            string title,
            string xAxisLabel,
            string yAxisLabel)
        {
            // generate plot arrays
            var x = feature.Concat(feature).ToArray();
            var y = predictions.Concat(labels).ToArray();

            // set up color arrays
            var colors1 = Enumerable.Repeat(1, labels.Length).ToArray();
            var colors2 = Enumerable.Repeat(2, labels.Length).ToArray();
            var c = colors1.Concat(colors2).ToArray();

            // plot the graph 
            var plot = new Scatterplot(title, xAxisLabel, yAxisLabel);
            plot.Compute(x, y, c);
            ScatterplotBox.Show(plot);
        }

        /// <summary>
        /// The main application entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            // get data
            Console.WriteLine("Loading data....");
            var path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\california_housing.csv"));
            var housing = Frame.ReadCsv(path, separators: ",");

            // set up a few series
            // ADD YOUR CODE HERE...

            // convert the house value range to thousands
            // ADD YOUR CODE HERE...

            // set up feature and label
            // ADD YOUR CODE HERE...

            // train the model
            // ADD YOUR CODE HERE...

            // show training results
            // ADD YOUR CODE HERE...

            // validate the model
            // ADD YOUR CODE HERE...

            // show validation results
            // ADD YOUR CODE HERE...

            // plot the results
            // ADD YOUR CODE HERE...

            Console.ReadLine();
        }
    }
}
