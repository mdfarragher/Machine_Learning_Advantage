using Accord.Controls;
using Accord.Math;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Models.Regression.Linear;
using Accord.Statistics.Visualizations;
using Deedle;
using System;
using System.IO;
using System.Linq;

namespace ml_csharp_lesson2
{
    /// <summary>
    /// The main application class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Plot a graph on screen.
        /// </summary>
        /// <param name="xSeries">The x-series to plot</param>
        /// <param name="ySeries">The y-series to plot</param>
        /// <param name="title">The plot title</param>
        /// <param name="xAxisLabel">The x-axis label</param>
        /// <param name="yAxisLabel">The y-axis label</param>
        public static void Plot(
            Series<int, double> xSeries,
            Series<int, double> ySeries,
            string title,
            string xAxisLabel,
            string yAxisLabel)
        {
            // generate plot arrays
            var x = xSeries.Values.ToArray();
            var y = ySeries.Values.ToArray();

            // plot the graph 
            var plot = new Scatterplot(title, xAxisLabel, yAxisLabel);
            plot.Compute(x, y);
            ScatterplotBox.Show(plot);
        }

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
            housing = housing.Where(kv => ((decimal)kv.Value["median_house_value"]) < 500000);

            // convert the house value range to thousands
            housing["median_house_value"] /= 1000;

            // ******************
            // ADD YOUR CODE HERE
            // ******************

            Console.ReadLine();
        }
    }
}
