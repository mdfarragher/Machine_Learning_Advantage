using System;
using CNTK;
using Pensar;

namespace ml_csharp_lesson1
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

            // report results
            Console.WriteLine($"{training_data.GetLength(0)} training digits loaded");
            Console.WriteLine($"{test_data.GetLength(0)} test digits loaded");

            // build features and labels
            var features = NetUtil.Var(new int[] { 28, 28, 1 }, DataType.Float);
            var labels = NetUtil.Var(new int[] { 10 }, DataType.Float);

            // build the network
            var network = features
                .Convolution2D(32, new int[] { 3, 3 }, activation: CNTKLib.ReLU)
                .Pooling(CNTK.PoolingType.Max, new int[] { 2, 2 }, new int[] { 2 })
                .Convolution2D(64, new int[] { 3, 3 }, activation: CNTKLib.ReLU)
                .Pooling(CNTK.PoolingType.Max, new int[] { 2, 2 }, new int[] { 2 })
                .Convolution2D(64, new int[] { 3, 3 }, activation: CNTKLib.ReLU)
                .Dense(64, CNTKLib.ReLU)
                .Dense(10, CNTKLib.Softmax)
                .ToNetwork();

            // print the network to the console
            Console.WriteLine("Neural Network architecture: ");
            Console.WriteLine(network.ToSummary());

            // set up the loss function and the classification error function
            var lossFunc = CNTKLib.CrossEntropyWithSoftmax(network.Output, labels);
            var errorFunc = CNTKLib.ClassificationError(network.Output, labels);

            // use the Adam learning algorithm
            var learner = network.GetAdamLearner(
                learningRateSchedule: (0.001, 1),
                momentumSchedule: (0.99, 1));

            // set up a trainer and an evaluator
            var trainer = network.GetTrainer(learner, lossFunc, errorFunc);
            var evaluator = network.GetEvaluator(errorFunc);

            // declare some variables
            var maxEpochs = 5;
            var batchSize = 64;
            var loss = 0.0;
            var error = 0.0;
            var batchCount = 0;

            // train the network during several epochs
            Console.WriteLine("Training the neural network....");
            for (int epoch = 0; epoch < maxEpochs; epoch++)
            {
                Console.Write($"Training epoch {epoch+1}/{maxEpochs}... ");

                // train the network using random batches
                loss = 0.0;
                error = 0.0;
                batchCount = 0;
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
                        loss += result.Loss;
                        error += result.Evaluation;
                        batchCount++;
                    });

                // show results
                loss /= batchCount;
                error /= batchCount;
                Console.Write($"training loss: {loss}, ");

                // test the network using batches
                error = 0.0;
                batchCount = 0;
                test_data.Batch(batchSize, (data, begin, end) =>
                {
                    // get the current batch for testing 
                    var featureBatch = features.GetBatch(test_data, begin, end);
                    var labelBatch = labels.GetBatch(test_labels, begin, end);

                    // test the network on the batch
                    error += evaluator.TestBatch(
                        new[] {
                            (features, featureBatch),
                            (labels,  labelBatch)
                        }
                    );
                    batchCount++;
                }
                );

                // show results
                error /= batchCount;
                Console.WriteLine($"validation error: {error}");
            }

            // show final results
            Console.WriteLine($"Final classification error on test data: {100 * error:0.00}%");
            Console.WriteLine($"Final accuracy on test data: {1 - error:0.0000}");

            Console.ReadLine();
        }
    }
}
