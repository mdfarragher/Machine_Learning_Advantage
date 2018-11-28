using CppInterop.LandmarkDetector;
using FaceAnalyser_Interop;
using FaceDetectorInterop;
using OpenCVWrappers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using UtilitiesOF;

namespace ml_csharp_lesson4
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// Initialize MainForm.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when MainForm loads.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // load the input image
            var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\images");
            var reader = new SequenceReader(folder, true);

            // process image
            var bitmap = ProcessImage(reader);

            // show new image
            pictureBox.Image = bitmap;
        }

        private Bitmap ProcessImage(SequenceReader reader)
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************

            return null; // replace this when done!
        }

        /// <summary>
        /// Get the top 3 emotions from the specified action units.
        /// </summary>
        /// <param name="actionUnits">The current action units.</param>
        /// <returns>The top 3 emotions corresponding to the action units.</returns>
        private IEnumerable<string> GetTopEmotions(IEnumerable<string> actionUnits)
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************

            return new[] { "Unknown emotion" }; // replace this when done!
        }
    }
}
