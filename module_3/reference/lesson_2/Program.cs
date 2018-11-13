using System;
using CNTK;
using Pensar;

namespace ml_csharp_lesson2
{
    class Program
    {
        /// <summary>
        /// The main entry point of the application.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        static void Main(string[] args)
        {
            Console.WriteLine("Loading data....");

            // unzip archive
            if (!System.IO.File.Exists("train_images.bin"))
            {
                DataUtil.Unzip(@"..\..\..\..\..\mnist_data.zip", ".");
            }

            // load training and test data
            var training_data = DataUtil.LoadBinary<float>("train_images.bin", 60000, 28 * 28);
            var test_data = DataUtil.LoadBinary<float>("test_images.bin", 10000, 28 * 28);
            var training_labels = DataUtil.LoadBinary<float>("train_labels.bin", 60000, 10);
            var test_labels = DataUtil.LoadBinary<float>("test_labels.bin", 10000, 10);

            // build features and labels
            var features = NetUtil.Var(new int[] { 28, 28 }, DataType.Float);
            var labels = NetUtil.Var(new int[] { 10 }, DataType.Float);

            // build the network
            var network = features
                .Dense(512, CNTKLib.ReLU)
                .Dense(10)
                .ToNetwork();

            // set up the loss function and the accuracy function
            var lossFunc = CNTKLib.CrossEntropyWithSoftmax(network.Output, labels);
            var errorFunc = CNTKLib.ClassificationError(network.Output, labels);

            // set up a trainer that uses an RMS algorithm
            var learner = network.GetRMSPropLearner(
                learningRateSchedule: 0.99,
                gamma: 0.95,
                inc: 2.0,
                dec: 0.5,
                max: 2.0,
                min: 0.5
            );

            var trainer = network.GetTrainer(learner, lossFunc, errorFunc);
            var evaluator = network.GetEvaluator(errorFunc);

            var maxEpochs = 10;
            var batchSize = 128;

            // train the network during several epochs
            Console.WriteLine("Training the neural network....");
            for (int epoch = 0; epoch < maxEpochs; epoch++)
            {
                Console.Write($"Training epoch {epoch+1}/{maxEpochs}... ");

                // train the network using random batches
                training_data.Index().Shuffle().Batch(batchSize, (indices, begin, end) =>
                    {
                        // get the current batch
                        var featureBatch = features.GetBatch(training_data, indices, begin, end);
                        var labelBatch = labels.GetBatch(training_labels, indices, begin, end);

                        // train the network on the batch
                        var result = trainer.TrainBatch(
                            new[] {
                                (features, featureBatch),
                                (labels,  labelBatch)
                            },
                            false
                        );
                        if (begin == 0)
                           Console.WriteLine($"loss: {result.Loss}");
                    });
            }

            var accuracy= 0.0;
            var numBatches = 0;

            // test the network using batches
            Console.WriteLine("Testing the neural network....");
            test_data.Batch(batchSize, (data, begin, end) =>
                {
                    // get the current batch
                    var featureBatch = features.GetBatch(test_data, begin, end);
                    var labelBatch = labels.GetBatch(test_labels, begin, end);

                    // test the network on the batch
                    accuracy += evaluator.TestBatch(
                        new[] {
                            (features, featureBatch),
                            (labels,  labelBatch)
                        }
                    );
                    numBatches++;
                }
            );

            // show results
            Console.WriteLine($"Final classification error on test data: {accuracy / numBatches}");

            Console.ReadLine();
        }
    }
}
