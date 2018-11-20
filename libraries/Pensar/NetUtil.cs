using System;
using System.Collections.Generic;
using System.Text;

namespace Pensar
{
    /// <summary>
    /// A collection of utilities to work with neural networks.
    /// </summary>
    public static class NetUtil
    {
        // the current compute device
        private static CNTK.DeviceDescriptor currentDevice = null;

        /// <summary>
        /// Get the current compute device. The library will use the first GPU
        /// it finds, and default to the CPU if no GPUs are found.
        /// </summary>
        /// <returns>The current compute device.</returns>
        public static CNTK.DeviceDescriptor CurrentDevice
        {
            get
            {
                if (currentDevice == null)
                {
                    currentDevice = CNTK.DeviceDescriptor.CPUDevice;
                    foreach (var gpuDevice in CNTK.DeviceDescriptor.AllDevices())
                    {
                        if (gpuDevice.Type == CNTK.DeviceKind.GPU)
                        {
                            currentDevice = gpuDevice;
                            break;
                        }
                    }
                }
                return currentDevice;
            }
        }

        /// <summary>
        /// Create an input/output variable for a neural network.
        /// </summary>
        /// <param name="shape">The shape of the variable.</param>
        /// <param name="dataType">The data type of the variable.</param>
        /// <returns>The created neural network variable.</returns>
        public static CNTK.Variable Var(
            IEnumerable<int> shape,
            CNTK.DataType dataType)
        {
            return CNTK.Variable.InputVariable(CNTK.NDShape.CreateNDShape(shape), dataType);
        }

        /// <summary>
        /// Add a dense layer to a neural network.
        /// </summary>
        /// <param name="input">The neural network to expand.</param>
        /// <param name="outputDim">The number of dimensions in the dense layer.</param>
        /// <param name="activation">The activation function in the dense layer.</param>
        /// <param name="outputName">The name of the layer.</param>
        /// <returns>The neural network with the dense layer added.</returns>
        public static CNTK.Variable Dense(
            this CNTK.Variable input,
            int outputDim,
            Func<CNTK.Variable, CNTK.Function> activation,
            string outputName = "")
        {
            return (CNTK.Variable)activation(Dense(input, outputDim, outputName));
        }

        /// <summary>
        /// Add a dense layer to a neural network.
        /// </summary>
        /// <param name="input">The neural network to expand.</param>
        /// <param name="outputDim">The number of dimensions in the dense layer.</param>
        /// <param name="outputName">The name of the layer.</param>
        /// <returns>The neural network with the dense layer added.</returns>
        public static CNTK.Variable Dense(
            this CNTK.Variable input,
            int outputDim,
            string outputName = "")
        {
            var shape = CNTK.NDShape.CreateNDShape(new int[] { outputDim, CNTK.NDShape.InferredDimension });
            var timesParam = new CNTK.Parameter(
                shape, 
                CNTK.DataType.Float, 
                CNTK.CNTKLib.GlorotUniformInitializer(
                    CNTK.CNTKLib.DefaultParamInitScale, 
                    CNTK.CNTKLib.SentinelValueForInferParamInitRank, 
                    CNTK.CNTKLib.SentinelValueForInferParamInitRank, 1), 
                CurrentDevice, 
                "timesParam_" + outputName);
            var timesFunction = CNTK.CNTKLib.Times(
                timesParam, 
                input, 
                1 /* output dimension */, 
                0 /* CNTK should infer the input dimensions */);
            var plusParam = new CNTK.Parameter(
                CNTK.NDShape.CreateNDShape(new int[] { CNTK.NDShape.InferredDimension }), 
                0.0f, 
                CurrentDevice, 
                "plusParam_" + outputName);
            var result = CNTK.CNTKLib.Plus(plusParam, timesFunction, outputName);
            return result;
        }

        /// <summary>
        /// Add a 2D convolution layer to a neural network.
        /// </summary>
        /// <param name="input">The neural network to expand.</param>
        /// <param name="outputChannels">The number of output channels</param>
        /// <param name="filterShape">The shape of the filter</param>
        /// <param name="padding">Use padding or not?</param>
        /// <param name="bias">Use bias or not?</param>
        /// <param name="strides">The stride lengths</param>
        /// <param name="activation">The activation function to use</param>
        /// <param name="outputName">The name of the layer.</param>
        /// <returns>The neural network with the convolution layer added.</returns>
        public static CNTK.Variable Convolution2D(
            this CNTK.Variable input,
            int outputChannels,
            int[] filterShape,
            bool padding = false,
            bool bias = true,
            int[] strides = null,
            Func<CNTK.Variable, string, CNTK.Function> activation = null,
            string outputName = "")
        {
            var convolution_map_size = new int[] {
                filterShape[0],
                filterShape[1],
                CNTK.NDShape.InferredDimension,
                outputChannels
            };
            if (strides == null)
            {
                strides = new int[] { 1 };
            }
            return Convolution(convolution_map_size, input, padding, bias, strides, activation, outputName);
        }

        /// <summary>
        /// Add a pooling layer to a neural network. 
        /// </summary>
        /// <param name="input">The neural network to expand</param>
        /// <param name="poolingType">The type of pooling to perform</param>
        /// <param name="windowShape">The shape of the pooling window</param>
        /// <param name="strides">The stride lengths</param>
        /// <returns>The neural network with the pooling layer added.</returns>
        public static CNTK.Variable Pooling(
            this CNTK.Variable input,
            CNTK.PoolingType poolingType,
            int[] windowShape,
            int[] strides)
        {
            return CNTK.CNTKLib.Pooling(input, poolingType, windowShape, strides);
        }

        /// <summary>
        /// Add a dropout layer to the neural network.
        /// </summary>
        /// <param name="input">The neural network to expand</param>
        /// <param name="dropoutRate">The dropout rate to use</param>
        /// <returns>The neural network with the dropout layer added</returns>
        public static CNTK.Variable Dropout(
            this CNTK.Variable input,
            double dropoutRate)
        {
            return CNTK.CNTKLib.Dropout(input, 0.5);
        }

        /// <summary>
        /// Multiply all tensor elements in the network by the given scalar.
        /// </summary>
        /// <typeparam name="T">The type of the scalar to multiply by</typeparam>
        /// <param name="input">The neural network</param>
        /// <param name="scalar">The scalar to multiply by</param>
        /// <returns>The neural network with the multiplication layer added</returns>
        public static CNTK.Variable MultiplyBy<T>(
            this CNTK.Variable input,
            T scalar)
        {
            var scalarTensor = CNTK.Constant.Scalar<T>(scalar, NetUtil.CurrentDevice);
            return CNTK.CNTKLib.ElementTimes(scalarTensor, input);
        }

        /// <summary>
        /// Add the VGG16 convolutional base to the network.
        /// </summary>
        /// <param name="input">The neural network</param>
        /// <param name="allowBlock5Finetuning">Indicates if block5 finetuning is allowed</param>
        /// <returns>The neural network with the VGG16 convolutional base added</returns>
        public static CNTK.Variable VGG16(
            this CNTK.Variable input, 
            bool allowBlock5Finetuning)
        {
            return DataUtil.VGG16.GetModel(input, allowBlock5Finetuning);
        }

        /// <summary>
        /// Cast a network layer to a Function.
        /// </summary>
        /// <param name="input">The neural network to expand.</param>
        /// <returns>The neural network layer cast to a Function instance.</returns>
        public static CNTK.Function ToNetwork(
            this CNTK.Variable input)
        {
            return (CNTK.Function)input;
        }

        /// <summary>
        /// Return a summary description of the neural network.
        /// </summary>
        /// <param name="model">The neural network to describe</param>
        /// <returns>A string description of the neural network</returns>
        public static string ToSummary(this CNTK.Function model)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("\tInput = " + model.Arguments[0].Shape.AsString());
            sb.Append(Environment.NewLine);
            for (int i = 0; i < model.Outputs.Count; i++)
            {
                sb.AppendFormat("\tOutput = " + model.Outputs[i].Shape.AsString());
                sb.Append(Environment.NewLine);
            }
            sb.Append(Environment.NewLine);

            var numParameters = 0;
            foreach (var x in model.Parameters())
            {
                var shape = x.Shape;
                var p = shape.TotalSize;
                sb.AppendFormat(string.Format("\tFilter Shape:{0,-30} Params:{1}", shape.AsString(), p));
                sb.Append(Environment.NewLine);
                numParameters += p;
            }
            sb.AppendFormat(string.Format("\tTotal Number of Parameters: {0:N0}", numParameters));
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        /// <summary>
        /// The binary accuracy loss function for binary classifiers.
        /// </summary>
        /// <param name="prediction">The prediction variable</param>
        /// <param name="labels">The label variable</param>
        /// <returns></returns>
        public static CNTK.Function BinaryAccuracy(CNTK.Variable prediction, CNTK.Variable labels)
        {
            var round_predictions = CNTK.CNTKLib.Round(prediction);
            var equal_elements = CNTK.CNTKLib.Equal(round_predictions, labels);
            var result = CNTK.CNTKLib.ReduceMean(equal_elements, CNTK.Axis.AllStaticAxes());
            return result;
        }

        /// <summary>
        /// The mean squared error loss function for linear models.
        /// </summary>
        /// <param name="prediction">The prediction variable</param>
        /// <param name="labels">The label variable</param>
        /// <returns></returns>
        public static CNTK.Function MeanSquaredError(CNTK.Variable prediction, CNTK.Variable labels)
        {
            var squared_errors = CNTK.CNTKLib.Square(CNTK.CNTKLib.Minus(prediction, labels));
            var result = CNTK.CNTKLib.ReduceMean(squared_errors, new CNTK.Axis(0)); // TODO -- allStaticAxes?
            return result;
        }

        /// <summary>
        /// The mean absolute error loss function for linear models.
        /// </summary>
        /// <param name="prediction">The prediction variable</param>
        /// <param name="labels">The label variable</param>
        /// <returns></returns>
        public static CNTK.Function MeanAbsoluteError(CNTK.Variable prediction, CNTK.Variable labels)
        {
            var absolute_errors = CNTK.CNTKLib.Abs(CNTK.CNTKLib.Minus(prediction, labels));
            var result = CNTK.CNTKLib.ReduceMean(absolute_errors, new CNTK.Axis(0)); // TODO -- allStaticAxes? 
            return result;
        }


        /// <summary>
        /// Get an RMSProp learner to train the network.
        /// </summary>
        /// <param name="input">The network to train.</param>
        /// <param name="learningRateSchedule">The learning rate schedule.</param>
        /// <param name="gamma">The gamma value.</param>
        /// <param name="inc">The inc value.</param>
        /// <param name="dec">The dec value.</param>
        /// <param name="max">The max value.</param>
        /// <param name="min">The min value.</param>
        /// <returns>An RMSProp learner to train the network.</returns>
        public static CNTK.Learner GetRMSPropLearner(
            this CNTK.Function input,
            double learningRateSchedule,
            double gamma,
            double inc,
            double dec,
            double max,
            double min)
        {
            var parameterVector = new CNTK.ParameterVector((System.Collections.ICollection)input.Parameters());
            return CNTK.CNTKLib.RMSPropLearner(
                parameterVector,
                new CNTK.TrainingParameterScheduleDouble(learningRateSchedule),
                gamma,
                inc,
                dec,
                max,
                min);
        }

        /// <summary>
        /// Get an Adam learner to train the network.
        /// </summary>
        /// <param name="input">The network to train.</param>
        /// <param name="learningRateSchedule">The learning rate schedule.</param>
        /// <param name="momentumSchedule">The moment schedule.</param>
        /// <param name="unitGain">The unit gain.</param>
        /// <returns>An Adamlearner to train the network.</returns>
        public static CNTK.Learner GetAdamLearner(
            this CNTK.Function input,
            (double, uint) learningRateSchedule,
            (double, uint) momentumSchedule,
            bool unitGain = true)
        {
            var parameterVector = new CNTK.ParameterVector((System.Collections.ICollection)input.Parameters());
            return CNTK.CNTKLib.AdamLearner(
                parameterVector,
                new CNTK.TrainingParameterScheduleDouble(learningRateSchedule.Item1, learningRateSchedule.Item2),
                new CNTK.TrainingParameterScheduleDouble(momentumSchedule.Item1, momentumSchedule.Item2),
                unitGain);
        }

        /// <summary>
        /// Get a trainer to train the network.
        /// </summary>
        /// <param name="input">The network to train.</param>
        /// <param name="lossFunc">The loss function to use.</param>
        /// <param name="evaluationFunc">The evaluation function to use.</param>
        /// <param name="learner">The learner to use.</param>
        /// <returns>A new trainer instance to train the network.</returns>
        public static CNTK.Trainer GetTrainer(
            this CNTK.Function input,
            CNTK.Learner learner,
            CNTK.Function lossFunc,
            CNTK.Function evaluationFunc)
        {
            return CNTK.CNTKLib.CreateTrainer(
                input, lossFunc,
                evaluationFunc,
                new CNTK.LearnerVector() { learner });
        }

        /// <summary>
        /// Get an evaluator to test the network.
        /// </summary>
        /// <param name="input">The network to test.</param>
        /// <param name="testFunc">The test function to use.</param>
        /// <returns>A new evaluator instance to test the network.</returns>
        public static CNTK.Evaluator GetEvaluator(
            this CNTK.Function input,
            CNTK.Function testFunc)
        {
            return CNTK.CNTKLib.CreateEvaluator(testFunc);
        }

        /// <summary>
        /// Train the network trainer on a batch.
        /// </summary>
        /// <param name="trainer">The trainer to use.</param>
        /// <param name="batch">The batch of features and labels to use.</param>
        /// <returns>A tuple of the current loss and evaluation values.</returns>
        public static (double Loss, double Evaluation) TrainBatch(
            this CNTK.Trainer trainer,
            (CNTK.Variable, CNTK.Value)[] batch,
            bool isSweepEndInArguments)
        {
            var dict = new Dictionary<CNTK.Variable, CNTK.Value>();
            foreach (var t in batch)
                dict.Add(t.Item1, t.Item2);

            trainer.TrainMinibatch(
                dict,
                false,
                CurrentDevice);

            return (
                Loss: trainer.PreviousMinibatchLossAverage(),
                Evaluation: trainer.PreviousMinibatchEvaluationAverage());
        }

        /// <summary>
        /// Train the network trainer on a batch.
        /// </summary>
        /// <param name="trainer">The trainer to use.</param>
        /// <param name="batch">The batch of features and labels to use.</param>
        /// <returns>A tuple of the current loss and evaluation values.</returns>
        public static (double Loss, double Evaluation) TrainBatch(
            this CNTK.Trainer trainer,
            (CNTK.Variable, CNTK.MinibatchData)[] batch)
        {
            var dict = new Dictionary<CNTK.Variable, CNTK.MinibatchData>();
            foreach (var t in batch)
                dict.Add(t.Item1, t.Item2);

            trainer.TrainMinibatch(
                dict,
                CurrentDevice);

            return (
                Loss: trainer.PreviousMinibatchLossAverage(),
                Evaluation: trainer.PreviousMinibatchEvaluationAverage());
        }


        /// <summary>
        /// Test the network evaluator on a batch.
        /// </summary>
        /// <param name="trainer">The evaluator to use.</param>
        /// <param name="batch">The batch of features and labels to use.</param>
        /// <returns>The current accuracy of the network.</returns>
        public static double TestBatch(
            this CNTK.Evaluator evaluator,
            (CNTK.Variable, CNTK.Value)[] batch)
        {
            var dict = new CNTK.UnorderedMapVariableValuePtr();
            foreach (var t in batch)
                dict.Add(t.Item1, t.Item2);

            return evaluator.TestMinibatch(
                dict,
                CurrentDevice);
        }

        /// <summary>
        /// Test the network evaluator on a batch.
        /// </summary>
        /// <param name="trainer">The evaluator to use.</param>
        /// <param name="batch">The batch of features and labels to use.</param>
        /// <returns>The current accuracy of the network.</returns>
        public static double TestBatch(
            this CNTK.Evaluator evaluator,
            (CNTK.Variable, CNTK.MinibatchData)[] batch)
        {
            var dict = new CNTK.UnorderedMapVariableMinibatchData();
            foreach (var t in batch)
                dict.Add(t.Item1, t.Item2);

            return evaluator.TestMinibatch(
                dict,
                CurrentDevice);
        }

        // *******************************************************************
        // Private utility functions
        // *******************************************************************

        /// <summary>
        /// Helper method to add a convolution layer to a neural network.
        /// </summary>
        /// <param name="convolutionMapSize"></param>
        /// <param name="input">The neural network to expand.</param>
        /// <param name="padding">Use padding or not?</param>
        /// <param name="bias">Use bias or not?</param>
        /// <param name="strides">The stride lengths</param>
        /// <param name="activation">The activation function to use</param>
        /// <param name="outputName">The name of the layer.</param>
        /// <returns></returns>
        private static CNTK.Function Convolution(
          int[] convolutionMapSize,
          CNTK.Variable input,
          bool padding,
          bool bias,
          int[] strides,
          Func<CNTK.Variable, string, CNTK.Function> activation = null,
          string outputName = "")
        {
            var W = new CNTK.Parameter(
              CNTK.NDShape.CreateNDShape(convolutionMapSize),
              CNTK.DataType.Float,
              CNTK.CNTKLib.GlorotUniformInitializer(
                  CNTK.CNTKLib.DefaultParamInitScale, 
                  CNTK.CNTKLib.SentinelValueForInferParamInitRank, 
                  CNTK.CNTKLib.SentinelValueForInferParamInitRank, 1),
              NetUtil.CurrentDevice, outputName + "_W");

            var result = CNTK.CNTKLib.Convolution(
                W, 
                input, 
                strides, 
                new CNTK.BoolVector(new bool[] { true }) /* sharing */, 
                new CNTK.BoolVector(new bool[] { padding }));

            if (bias)
            {
                var num_output_channels = convolutionMapSize[convolutionMapSize.Length - 1];
                var b_shape = ConcatenateArrays(MakeOnesArray(convolutionMapSize.Length - 2), new int[] { num_output_channels });
                var b = new CNTK.Parameter(b_shape, 0.0f, NetUtil.CurrentDevice, outputName + "_b");
                result = CNTK.CNTKLib.Plus(result, b);
            }

            if (activation != null)
            {
                result = activation(result, outputName);
            }
            return result;
        }

        /// <summary>
        /// Concatenate arrays.
        /// </summary>
        /// <typeparam name="T">The type of array element</typeparam>
        /// <param name="arguments">The arrays to concatenate</param>
        /// <returns>The concatenated array</returns>
        private static T[] ConcatenateArrays<T>(params T[][] arguments) where T : struct
        {
            var list = new List<T>();
            for (int i = 0; i < arguments.Length; i++)
            {
                list.AddRange(arguments[i]);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Create an array filled with ones.
        /// </summary>
        /// <param name="numOnes">The number of ones to create</param>
        /// <returns>A new array filled with the specified number of ones</returns>
        private static int[] MakeOnesArray(int numOnes)
        {
            var ones = new int[numOnes];
            for (int i = 0; i < numOnes; i++) { ones[i] = 1; }
            return ones;
        }


    }
}
