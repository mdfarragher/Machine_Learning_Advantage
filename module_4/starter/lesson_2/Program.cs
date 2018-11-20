using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CNTK;
using Pensar;

namespace ml_csharp_lesson2
{
    /// <summary>
    /// The plot window class for showing to show the training curve.
    /// </summary>
    class PlotWindow : System.Windows.Window
    {
        /// <summary>
        /// Construct a new plot window.
        /// </summary>
        /// <param name="results">The data to plot.</param>
        public PlotWindow(List<List<double>> results)
        {
            var plotModel = new OxyPlot.PlotModel();
            plotModel.Title = "Cats And Dogs";
            plotModel.LegendPosition = OxyPlot.LegendPosition.BottomRight;

            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Title = "Accuracy",
                Minimum = 0,
                Maximum = 1,
            });
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis()
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Minimum = 0,
                Maximum = results[0].Count + 1,
                Title = "Epochs"
            });

            var labels = new string[] { "Train Set", "Validation Set" };
            var markerTypes = new OxyPlot.MarkerType[] { OxyPlot.MarkerType.Plus, OxyPlot.MarkerType.Circle };
            for (int row = 0; row < results.Count; row++)
            {
                var scatterSeries = new OxyPlot.Series.ScatterSeries()
                {
                    MarkerType = markerTypes[row],
                    MarkerStroke = OxyPlot.OxyColors.Blue,
                    MarkerFill = OxyPlot.OxyColors.Black
                };
                scatterSeries.ItemsSource = results[row].Select((value, index) => new OxyPlot.Series.ScatterPoint(index + 1, value));
                scatterSeries.Title = labels[row];
                plotModel.Series.Add(scatterSeries);
            }
            var plotView = new OxyPlot.Wpf.PlotView();
            plotView.Model = plotModel;
            Title = "Chart";
            Content = plotView;
        }
    }

    /// <summary>
    /// The main application class.
    /// </summary>
    class Program
    {
        // some training constants
        const int trainingSetSize = 1000;
        const int validationSetSize = 500;
        const int testSetSize = 500;
        const int imageWidth = 150;
        const int imageHeight = 150;
        const int numChannels = 3;
        const int maxEpochs = 100;
        const int batchSize = 32;

        /// <summary>
        /// Create the mapping files for features and labels
        /// </summary>
        static void CreateMapFiles()
        {
            var filenames = new string[] { "train_map.txt", "validation_map.txt", "test_map.txt" };
            var num_entries = new int[] { trainingSetSize, validationSetSize, testSetSize };
            var counter = 0;
            for (int j = 0; j < filenames.Length; j++)
            {
                var filePath = filenames[j];
                using (var dstFile = new StreamWriter(filePath))
                {
                    for (int i = 0; i < num_entries[j]; i++)
                    {
                        var catPath = Path.Combine("cat", $"{counter}.jpg");
                        var dogPath = Path.Combine("dog", $"{counter}.jpg");
                        dstFile.WriteLine($"{catPath}\t0");
                        dstFile.WriteLine($"{dogPath}\t1");
                        counter++;
                    }
                    Console.WriteLine($"  Created file: {filePath}");
                }
            }
        }

        /// <summary>
        /// The main entry point of the application.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        [STAThread]
        static void Main(string[] args)
        {
            // unzip archive
            if (!Directory.Exists("cat"))
            {
                Console.WriteLine("Unpacking data....");
                DataUtil.Unzip(@"..\..\..\..\..\catsanddogs.zip", ".");
            }

            // create mapping files
            if (!File.Exists("train_map.txt"))
            {
                Console.WriteLine("Creating mapping files...");
                CreateMapFiles();
            }

            // ******************
            // ADD YOUR CODE HERE
            // ******************

            Console.ReadLine();
        }
    }
}
