using Accord.Controls;
using Accord.Math;
using Accord.Math.Optimization.Losses;
using Accord.Statistics;
using Accord.Statistics.Models.Regression.Linear;
using Accord.Statistics.Visualizations;
using ConsoleTables;
using Deedle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ml_csharp_lesson3
{
    /// <summary>
    /// The main application class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Return a collection of bins.
        /// </summary>
        /// <param name="start">The starting bin value</param>
        /// <param name="end">The ending bin value</param>
        /// <returns>A collection of tuples that represent each bin</returns>
        private static IEnumerable<(int Min, int Max)> Bins(int start, int end)
        {
            return Enumerable.Range(start, end - start + 1)
                .Select(v => (Min: v, Max: v + 1));
        }

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
        /// Print the correlation matrix on the console.
        /// </summary>
        /// <param name="data">The data frame the matrix is based on</param>
        /// <param name="matrix">The correlation matrix to print</param>
        private static void Print(Frame<int, string> data, double[,] matrix)
        {
            // set up table header
            var table = new ConsoleTable(" "); 
            var columns = data.ColumnKeys.ToArray();
            table.AddColumn(columns);

            // add matrix data
            var step = matrix.GetLength(1);
            var colIndex = 0;
            for (var i=0; i < matrix.Length; i+=step)
            {
                var values = matrix.Cast<double>().Skip(i).Take(step).Select(v => v.ToString("0.0")).ToList();
                values.Insert(0, columns[colIndex++]);
                table.AddRow(values.ToArray());
            }

            // show the correlation matrix
            table.Write();
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

            // create the rooms_per_person feature
            housing.AddColumn("rooms_per_person",
               (housing["total_rooms"] / housing["population"]).Select(v => v.Value <= 4.0 ? v.Value : 4.0));

            // calculate the correlation matrix
            var correlation = Measures.Correlation(housing.ToArray2D<double>());

            // show the correlation matrix
            Print(housing, correlation);

            // calculate binned latitudes
            var binned_latitude =
                from l in housing["latitude"].Values
                let bin = (from b in Bins(32, 41)
                           where l >= b.Min && l < b.Max
                           select b)
                select bin.First().Min;

            // add one-hot encoding columns
            foreach (var i in Enumerable.Range(32, 10))
            {
                housing.AddColumn($"latitude {i}-{i + 1}",
                    from l in binned_latitude
                    select l == i ? 1 : 0);
            }

            // drop the latitude column
            housing.DropColumn("latitude");

            // show the data frame on the console
            housing.Print();

            // calculate rooms_per_person histogram
            var histogram = new Histogram();
            histogram.Compute(housing["rooms_per_person"].Values.ToArray(), 0.1);

            // plot the histogram
            Plot(histogram, "Histogram", "Rooms per person", "Number of housing blocks");

            Console.ReadLine();
        }
    }
}
