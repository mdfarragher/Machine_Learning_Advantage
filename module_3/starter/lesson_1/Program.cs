using Accord.Controls;
using Accord.Math;
using Accord.Neuro;
using Accord.Neuro.Learning;
using Accord.Statistics.Visualizations;
using Deedle;
using System;
using System.Collections.Generic;
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
        /// Plot the training errors.
        /// </summary>
        /// <param name="trainingErrors">The traininer errors to plot</param>
        /// <param name="title">The chart title</param>
        /// <param name="xAxisLabel">The chart x-ais label</param>
        /// <param name="yAxisLabel">The chart y-axis label</param>
        private static void Plot(
            List<double> trainingErrors,
            string title,
            string xAxisLabel,
            string yAxisLabel)
        {
            var epochs = trainingErrors.Count();
            var x = Enumerable.Range(0, epochs).Select(v => (double)v).ToArray();
            var y = trainingErrors.ToArray();
            var plot = new Scatterplot(title, xAxisLabel, yAxisLabel);
            plot.Compute(x, y);
            ScatterplotBox.Show(plot);
        }

        /// <summary>
        /// Plot the training and validation errors.
        /// </summary>
        /// <param name="trainingErrors">The traininer errors to plot</param>
        /// <param name="validationErrors">The validation errors to plot</param>
        /// <param name="title">The chart title</param>
        /// <param name="xAxisLabel">The chart x-ais label</param>
        /// <param name="yAxisLabel">The chart y-axis label</param>
        private static void Plot(
            List<double> trainingErrors,
            List<double> validationErrors,
            string title,
            string xAxisLabel,
            string yAxisLabel)
        {
            var epochs = trainingErrors.Count();
            var x = Enumerable.Range(0, epochs).Concat(Enumerable.Range(0, epochs)).Select(v => (double)v).ToArray();
            var y = trainingErrors.Concat(validationErrors).ToArray();
            var sets = Enumerable.Repeat(1, epochs).Concat(Enumerable.Repeat(2, epochs)).ToArray();
            var plot = new Scatterplot(title, xAxisLabel, yAxisLabel);
            plot.Compute(x, y, sets);
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

            // shuffle the frame
            var rnd = new Random();
            var indices = Enumerable.Range(0, housing.Rows.KeyCount).OrderBy(v => rnd.NextDouble());
            housing = housing.IndexRowsWith(indices).SortRowsByKey();

            // convert the house value range to thousands
            housing["median_house_value"] /= 1000;

            // create training, validation, and test partitions
            var training = housing.Rows[Enumerable.Range(0, 12000)];
            var validation = housing.Rows[Enumerable.Range(12000, 2500)];
            var test = housing.Rows[Enumerable.Range(14500, 2500)];

            // ******************
            // ADD YOUR CODE HERE
            // ******************

            Console.ReadLine();
        }
    }
}
