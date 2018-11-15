using System;
using System.Collections.Generic;
using System.Linq;
using CNTK;
using Pensar;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace ml_csharp_lesson3
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
            // ******************
            // ADD YOUR CODE HERE
            // ******************

            Console.ReadLine();
        }
    }
}
