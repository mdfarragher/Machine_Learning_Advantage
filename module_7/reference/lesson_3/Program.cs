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
            // load images
            Console.WriteLine("Loading content and style images...");
            var contentImage = StyleTransfer.LoadImage(contentImagePath, imageWidth, imageHeight);
            var styleImage = StyleTransfer.LoadImage(styleImagePath, imageWidth, imageHeight);

            // create the feature variable
            var featureVariable = CNTK.Variable.InputVariable(new int[] { imageWidth, imageHeight, 3 }, CNTK.DataType.Float);

            // create the neural network base (just the content and style layers)
            Console.WriteLine("Creating VGG19 style transfer model...");
            var model = featureVariable
                .VGG19(freeze: true)
                .StyleTransferBase();

            // calculate the labels
            Console.WriteLine("Calculating output labels...");
            var labels = StyleTransfer.CalculateLabels(model, contentImage, styleImage);

            // add the dream layer
            model = model
                .DreamLayer(contentImage, imageWidth, imageHeight);

            // show the model summary
            Console.WriteLine(model.ToSummary());

            // create the label variable
            var contentAndStyle = model.GetContentAndStyleLayers();
            var labelVariable = new List<CNTK.Variable>();
            for (int i = 0; i < labels.Length; i++)
            {
                var shape = contentAndStyle[i].Shape;
                var input_variable = CNTK.Variable.InputVariable(shape, CNTK.DataType.Float, "content_and_style_" + i);
                labelVariable.Add(input_variable);
            }

            // create the loss function
            var lossFunction = StyleTransfer.CreateLossFunction(model, contentAndStyle, labelVariable);

            // set up an AdamLearner
            var learner = model.GetAdamLearner(10, 0.95);

            // get the model trainer
            var trainer = model.GetTrainer(learner, lossFunction, lossFunction);

            // create the batch to train on
            var trainingBatch = StyleTransfer.CreateBatch(lossFunction, labels);

            // train the model
            Console.WriteLine("Training the model...");
            var numEpochs = 300;
            for (int i = 0; i < numEpochs; i++)
            {
                trainer.TrainMinibatch(trainingBatch, true, NetUtil.CurrentDevice);
                if (i % 50 == 0)
                    Console.WriteLine($"epoch {i}, training loss = {trainer.PreviousMinibatchLossAverage()}");
            }

            // create a batch to evaluate the model on
            var evaluationBatch = StyleTransfer.CreateBatch(model, labels);

            // infer the image from the model
            Console.WriteLine("Inferring transformed image...");
            var img = model.InferImage(evaluationBatch);

            // show image
            ShowImage(img);
        }
    }
}
