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

            // shuffle row indices
            var rnd = new Random();
            var indices = Enumerable.Range(0, housing.Rows.KeyCount).OrderBy(v => rnd.NextDouble());

            // shuffle the frame using the indices
            housing = housing.IndexRowsWith(indices).SortRowsByKey();

            // create training, validation, and test frames
            var training = housing.Rows[Enumerable.Range(0, 12000)];
            var validation = housing.Rows[Enumerable.Range(12000, 2500)];
            var test = housing.Rows[Enumerable.Range(14500, 2500)];

            // plot the training, validation, and test data
            // to check if it looks like California
            Plot(training["longitude"], training["latitude"], "Training data", "longitude", "latitude");
            Plot(validation["longitude"], validation["latitude"], "Validation data", "longitude", "latitude");
            Plot(test["longitude"], test["latitude"], "Test data", "longitude", "latitude");

            // set up training features and labels
            var training_features = training["median_income"].Values.ToArray();
            var training_labels = training["median_house_value"].Values.ToArray();

            // train the model
            Console.WriteLine("Training model....");
            var learner = new OrdinaryLeastSquares();
            var model = learner.Learn(training_features, training_labels);

            // show results
            Console.WriteLine("TRAINING RESULTS");
            Console.WriteLine($"Slope:       {model.Slope}");
            Console.WriteLine($"Intercept:   {model.Intercept}");
            Console.WriteLine();

            // plot the regression line on the training data
            var training_predictions = model.Transform(training_features);
            Plot(training_features, training_labels, training_predictions, "Training Regression", "Median Income", "Median house value");

            // set up validation features and labels
            var validation_features = validation["median_income"].Values.ToArray();
            var validation_labels = validation["median_house_value"].Values.ToArray();

            // validate the model
            var validation_predictions = model.Transform(validation_features);
            var validation_rmse = Math.Sqrt(new SquareLoss(validation_labels).Loss(validation_predictions));

            // show validation results
            var validation_range = Math.Abs(validation_labels.Max() - validation_labels.Min());
            Console.WriteLine("VALIDATION RESULTS");
            Console.WriteLine($"Label range: {validation_range}");
            Console.WriteLine($"RMSE:        {validation_rmse} {validation_rmse / validation_range * 100:0.00}%");
            Console.WriteLine();

            // plot the regression line on the validation data
            Plot(validation_features, validation_labels, validation_predictions, "Validation Regression", "Median Income", "Median house value");

            // set up test features and labels
            var test_features = test["median_income"].Values.ToArray();
            var test_labels = test["median_house_value"].Values.ToArray();

            // validate the model
            var test_predictions = model.Transform(test_features);
            var test_rmse = Math.Sqrt(new SquareLoss(test_labels).Loss(test_predictions));

            // show validation results
            var test_range = Math.Abs(test_labels.Max() - test_labels.Min());
            Console.WriteLine("TEST RESULTS");
            Console.WriteLine($"Label range: {test_range}");
            Console.WriteLine($"RMSE:        {test_rmse} {test_rmse / test_range * 100:0.00}%");
            Console.WriteLine();

            // plot the regression line on the test data
            Plot(test_features, test_labels, test_predictions, "Test Regression", "Median Income", "Median house value");

            Console.ReadLine();
        }
    }
}
