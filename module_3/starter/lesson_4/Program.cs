using System;
using System.Collections.Generic;
using System.Linq;
using CNTK;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Pensar;

namespace ml_csharp_lesson4
{
    /// <summary>
    /// The Plot class encapsulates a plotting window. 
    /// </summary>
    class Plot : System.Windows.Window
    {
        /// <summary>
        /// Construct a new instance of the class.
        /// </summary>
        /// <param name="title">The plot title.</param>
        /// <param name="results">The data to plot.</param>
        public Plot(string title, List<List<double>> results)
        {
            // set up plot model
            var plotModel = new OxyPlot.PlotModel();
            plotModel.Title = title;

            // set up axes and colors
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis() { Position = OxyPlot.Axes.AxisPosition.Left, Title = "Error" });
            plotModel.Axes.Add(new OxyPlot.Axes.LinearAxis() { Position = OxyPlot.Axes.AxisPosition.Bottom, Title = "Epochs" });
            var colors = new OxyPlot.OxyColor[] { OxyPlot.OxyColors.Blue, OxyPlot.OxyColors.Green, OxyPlot.OxyColors.Red, OxyPlot.OxyColors.Black };

            // set up lines
            for (int i = 0; i < results.Count; i++)
            {
                var lineSeries = new OxyPlot.Series.LineSeries();
                lineSeries.ItemsSource = results[i].Select((value, index) => new OxyPlot.DataPoint(index, value));
                lineSeries.Title = string.Format("KFold {0}/{1}", i + 1, results.Count);
                //lineSeries.Color = colors[i];
                plotModel.Series.Add(lineSeries);
            }

            var plotView = new OxyPlot.Wpf.PlotView();
            plotView.Model = plotModel;

            Title = title;
            Content = plotView;
        }
    }

    /// <summary>
    /// The main application class.
    /// </summary>
    class Program
    {
        // local members
        private static CNTK.Variable features;
        private static CNTK.Variable labels;
        private static CNTK.Trainer trainer;
        private static CNTK.Evaluator evaluator;

        /// <summary>
        /// Create the neural network for this app.
        /// </summary>
        /// <returns>The neural network to use</returns>
        public static CNTK.Function CreateNetwork()
        {
            // ***********************************
            // ADD YOUR NETWORK CREATION CODE HERE
            // ***********************************

            return null; // remove this when done!
        }

        /// <summary>
        /// The main entry point of the application.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        [STAThread]
        public static void Main(string[] args)
        {
            // ****************************
            // ADD YOUR REMAINING CODE HERE
            // ****************************

            Console.ReadLine();
        }
    }
}
