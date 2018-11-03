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

            // filter all houses worth more than $500k
            housing = housing.Where(kv => ((decimal)kv.Value["median_house_value"]) < 500000);

            // set up a few series
            var total_rooms = housing["total_rooms"];
            var median_house_value = housing["median_house_value"];
            var median_income = housing["median_income"];

            // convert the house value range to thousands
            median_house_value /= 1000;

            // set up feature and label
            var feature = median_income.Values.ToArray();
            var labels = median_house_value.Values.ToArray();

            // train the model
            Console.WriteLine("Training model....");
            var learner = new OrdinaryLeastSquares();
            var model = learner.Learn(feature, labels);

            // show training results
            Console.WriteLine($"Slope:       {model.Slope}");
            Console.WriteLine($"Intercept:   {model.Intercept}");

            // validate the model
            var predictions = model.Transform(feature);
            var rmse = Math.Sqrt(new SquareLoss(labels).Loss(predictions));

            // show validation results
            var range = Math.Abs(labels.Max() - labels.Min());
            Console.WriteLine($"Label range: {range}");
            Console.WriteLine($"RMSE:        {rmse} {rmse / range * 100:0.00}%");

            // plot the results
            Plot(feature, labels, predictions, "Linear Regression Model", "Median Income", "Median house value");

            Console.ReadLine();
        }
    }
}
