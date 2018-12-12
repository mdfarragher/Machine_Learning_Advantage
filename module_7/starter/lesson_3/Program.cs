using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using Pensar;

namespace ml_csharp_lesson3
{
    /// <summary>
    /// The application class.
    /// </summary>
    class Program
    {
        // paths to the content and style images
        static readonly string contentImagePath = "content.png";
        static readonly string styleImagePath = "style.png";

        // the width and height to resize the images to
        static readonly int imageHeight = 400;
        static readonly int imageWidth = 381;

        /// <summary>
        /// Show the inferred image.
        /// </summary>
        /// <param name="imageData">The image data of the inferred image.</param>
        static void ShowImage(byte[] imageData)
        {
            var mat = new OpenCvSharp.Mat(imageHeight, imageWidth, OpenCvSharp.MatType.CV_8UC3, imageData, 3 * imageWidth);
            Cv2.ImShow("Image With Style Transfer", mat);
            Cv2.WaitKey(0);
        }

        /// <summary>
        /// The main application entry point.
        /// </summary>
        /// <param name="args">The command line arguments</param>
        static void Main(string[] args)
        {
            // ******************
            // ADD YOUR CODE HERE
            // ******************
        }
    }
}
