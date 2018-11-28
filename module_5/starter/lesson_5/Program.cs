using CppInterop.LandmarkDetector;
using FaceDetectorInterop;
using GazeAnalyser_Interop;
using OpenCVWrappers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using UtilitiesOF;

namespace ml_csharp_lesson5
{
    /// <summary>
    /// The main program.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The game form.
        /// </summary>
        static GameForm form = null;

        /// <summary>
        /// The face model to use. 
        /// </summary>
        static FaceModelParameters faceModel = null;

        /// <summary>
        /// The sequence reader to read webcam images with. 
        /// </summary>
        static SequenceReader reader = null;

        /// <summary>
        /// The face detector to use.
        /// </summary>
        static FaceDetector faceDetector = null;

        /// <summary>
        /// The landmark detector to use. 
        /// </summary>
        static CLNF landmarkDetector = null;

        /// <summary>
        /// The gaze analyser to use. 
        /// </summary>
        static GazeAnalyserManaged gazeAnalyser = null;

        /// <summary>
        /// The last known position of the face. 
        /// </summary>
        static Rect currentFace = new Rect();

        /// <summary>
        /// The number of the current camera frame.
        /// </summary>
        static int frameIndex = 0;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread] 
        static void Main()
        {
            // initialize application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // set up the game form
            form = new GameForm();
            form.OnGazeMove += new OnGazeMoveDelegate(OnGazeMove);

            // ******************
            // ADD YOUR CODE HERE
            // ******************
        }

        /// <summary>
        /// Calculate the current gaze point.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnGazeMove(object sender, GazeEventArgs e)
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************
        }
    }
}
