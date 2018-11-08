using Accord.Controls;
using Accord.Math;
using Accord.Math.Optimization.Losses;
using Accord.Statistics;
using Accord.Statistics.Analysis;
using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Fitting;
using Accord.Statistics.Models.Regression.Linear;
using Accord.Statistics.Visualizations;
using Deedle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ml_csharp_lesson1
{
    /// <summary>
    /// The lesson 6B code.
    /// </summary>
    public static class Lesson1b
    {
        /// <summary>
        /// Plot a histogram.
        /// </summary>
        /// <param name="histogram">The histogram to plot</param>
        /// <param name="title">The plot title</param>
        /// <param name="xAxisLabel">The x-axis label</param>
        /// <param name="yAxisLabel">The y-axis label</param>
        private static void Plot(Histogram histogram, string title, string xAxisLabel, string yAxisLabel)
        {
            var x = new List<double>();
            var y = new List<double>();
            for (int i = 0; i < histogram.Values.Length; i++)
            {
                var xcor = histogram.Bins[i].Range.Min;
                x.AddRange(from n in Enumerable.Range(0, histogram.Values[i]) select xcor);
                y.AddRange(from n in Enumerable.Range(0, histogram.Values[i]) select n * 1.0);
            }
            var plot = new Scatterplot(title, xAxisLabel, yAxisLabel);
            plot.Compute(x.ToArray(), y.ToArray());
            ScatterplotBox.Show(plot);
        }

        /// <summary>
        /// Plot the given scatterplot on screen.
        /// </summary>
        /// <param name="plot">The scatterplot to plot</param>
        private static void Plot(Scatterplot plot)
        {
            var box = ScatterplotBox.Show(plot);
            var callback = new Action(() =>
            {
                box.SetLinesVisible(true);
                box.SetSymbolSize(0);
                box.SetScaleTight(true);
            });
            System.Threading.Thread.Sleep(100);
            box.Invoke(callback);
        }

        /// <summary>
        /// Run the lesson. 
        /// </summary>
        public static void Run()
        {
            // get data
            Console.WriteLine("Loading data....");
            var path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\california_housing.csv"));
            var housing = Frame.ReadCsv(path, separators: ",");
            housing = housing.Where(kv => ((decimal)kv.Value["median_house_value"]) < 500000);

            // create the median_high_house_value feature
            housing.AddColumn("median_high_house_value",
                housing["median_house_value"].Select(v => v.Value >= 265000 ? 1.0 : 0.0));

            // shuffle the frame
            var rnd = new Random();
            var indices = Enumerable.Range(0, housing.Rows.KeyCount).OrderBy(v => rnd.NextDouble());
            housing = housing.IndexRowsWith(indices).SortRowsByKey();

            // create training, validation, and test frames
            var training = housing.Rows[Enumerable.Range(0, 12000)];
            var validation = housing.Rows[Enumerable.Range(12000, 2500)];
            var test = housing.Rows[Enumerable.Range(14500, 2500)];

            // build the list of features we're going to use
            var columns = new string[] {
                "latitude",
                "longitude",
                "housing_median_age",
                "total_rooms",
                "total_bedrooms",
                "population",
                "households",
                "median_income" };

            // train the model using a logistic regressor
            var learner = new IterativeReweightedLeastSquares<LogisticRegression>()
            {
                MaxIterations = 100
            };
            var regression = learner.Learn(
                training.Columns[columns].ToArray2D<double>().ToJagged(),
                training["median_high_house_value"].Values.ToArray());

            // get probabilities
            var features_validation = validation.Columns[columns].ToArray2D<double>().ToJagged();
            var label_validation = validation["median_high_house_value"].Values.ToArray();
            var probabilities = regression.Probability(features_validation);

            // calculate the histogram of probabilities
            var histogram = new Histogram();
            histogram.Compute(probabilities, 0.05);

            // draw the histogram
            Plot(histogram, "Probability histogram", "prediction", "count");

            // get predictions and actuals
            var predictions = regression.Decide(features_validation);
            var actuals = label_validation.Select(v => v == 1.0 ? true : false).ToArray();

            // create confusion matrix
            var confusion = new ConfusionMatrix(predictions, actuals);

            // display classification scores
            Console.WriteLine($"True Positives:  {confusion.TruePositives}");
            Console.WriteLine($"True Negatives:  {confusion.TrueNegatives}");
            Console.WriteLine($"False Positives: {confusion.FalsePositives}");
            Console.WriteLine($"False Negatives: {confusion.FalseNegatives}");
            Console.WriteLine();

            // display accuracy, precision, and recall
            Console.WriteLine($"Accuracy:        {confusion.Accuracy}");
            Console.WriteLine($"Precision:       {confusion.Precision}");
            Console.WriteLine($"Recall:          {confusion.Recall}");
            Console.WriteLine();

            // display TPR and FPR
            Console.WriteLine($"TPR:             {confusion.Sensitivity}");
            Console.WriteLine($"FPR:             {confusion.FalsePositiveRate}");
            Console.WriteLine();

            // calculate roc curve
            var roc = new ReceiverOperatingCharacteristic(
                actuals,
                predictions.Select(v => v ? 1 : 0).ToArray());
            roc.Compute(100);

            // generate the scatter plot
            var rocPlot = roc.GetScatterplot(true);

            // show roc curve
            Plot(rocPlot);

            // show the auc
            Console.WriteLine($"AUC:             {roc.Area}");
        }
    }
}
