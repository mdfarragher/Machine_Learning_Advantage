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
        /// <returns>The neural network with the dense layer added.</returns>
        public static CNTK.Variable Dense(
            this CNTK.Variable input,
            int outputDim,
            Func<CNTK.Variable, CNTK.Function> activation)
        {
            return (CNTK.Variable)activation(Dense(input, outputDim));
        }

        /// <summary>
        /// Add a dense layer to a neural network.
        /// </summary>
        /// <param name="input">The neural network to expand.</param>
        /// <param name="outputDim">The number of dimensions in the dense layer.</param>
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
        /// <param name="gamma">The gamma value.</param>
        /// <param name="inc">The inc value.</param>
        /// <param name="dec">The dec value.</param>
        /// <param name="max">The max value.</param>
        /// <param name="min">The min value.</param>
        /// <returns>An RMSProp learner to train the network.</returns>
        public static CNTK.Learner GetAdamLearner(
            this CNTK.Function input,
            (double, uint) learningRateSchedule,
            (double, uint) momentumSchedule,
            bool unitGain)
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

    }
}
