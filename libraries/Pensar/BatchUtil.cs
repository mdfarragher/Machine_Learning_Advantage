using System;
using System.Collections.Generic;
using System.Text;

namespace Pensar
{
    /// <summary>
    /// A collection of utilities for working with batches. 
    /// </summary>
    public static class BatchUtil
    {
        /// <summary>
        /// Create an index for the given data array.
        /// </summary>
        /// <param name="data">The data array to use.</param>
        /// <returns>An array instance with the numbers 0..N, with N the size of the data array.</returns>
        public static int[] Index(this float[][] data)
        {
            var array = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                array[i] = i;
            }
            return array;
        }

        /// <summary>
        /// Swap two elements in an array.
        /// </summary>
        /// <typeparam name="T">The type of array elements</typeparam>
        /// <param name="array">The array to swap elements in</param>
        /// <param name="n">The index of the first element to swap</param>
        /// <param name="k">The index of the second element to swap</param>
        private static void Swap<T>(T[] array, int n, int k)
        {
            var temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }

        /// <summary>
        /// Shuffle the given array.
        /// </summary>
        /// <param name="array">The array to shuffle.</param>
        /// <returns>The shuffled array.</returns>
        public static int[] Shuffle(this int[] array)
        {
            var rng = new Random();
            var n = array.Length;
            while (n > 1)
            {
                var k = rng.Next(n--);
                Swap(array, n, k);
            }
            return array;
        }

        /// <summary>
        /// Partition the indices into a set of batches and call an action on each batch.
        /// </summary>
        /// <param name="indices">The indices to use.</param>
        /// <param name="batchSize">The size of each batch.</param>
        /// <param name="action">The action to perform on each batch.</param>
        public static void Batch(
            this int[] indices,
            int batchSize,
            Action<int[], int, int> action)
        {
            var begin = 0;
            while (begin < indices.Length)
            {
                var end = Math.Min(begin + batchSize, indices.Length);
                action(indices, begin, end);
                begin = end;

            }
        }

        /// <summary>
        /// Partition the data array into a set of batches and call an action on each batch.
        /// </summary>
        /// <param name="data">The data array to use.</param>
        /// <param name="batchSize">The size of each batch.</param>
        /// <param name="action">The action to perform on each batch.</param>
        public static void Batch(
            this float[][] data,
            int batchSize,
            Action<float[][], int, int> action)
        {
            var begin = 0;
            while (begin < data.Length)
            {
                var end = Math.Min(begin + batchSize, data.Length);
                action(data, begin, end);
                begin = end;

            }
        }

        /// <summary>
        /// Get a batch from the given variable.
        /// </summary>
        /// <param name="variable">The variable to use.</param>
        /// <param name="source">The variable data.</param>
        /// <param name="indices">The array of data indices to use.</param>
        /// <param name="begin">The first index to use.</param>
        /// <param name="end">The last index to use.</param>
        /// <returns>A batch of values taken from the given variable.</returns>
        public static CNTK.Value GetBatch(
            this CNTK.Variable variable,
            float[][] source,
            int[] indices,
            int begin,
            int end)
        {
            var num_indices = end - begin;
            var row_length = variable.Shape.TotalSize;
            var result = new CNTK.NDArrayView[num_indices];

            var row_index = 0;
            for (var index = begin; index != end; index++)
            {
                var dataBuffer = source[indices[index]];
                var ndArrayView = new CNTK.NDArrayView(variable.Shape, dataBuffer, CNTK.DeviceDescriptor.CPUDevice, true);
                result[row_index++] = ndArrayView;
            }
            return CNTK.Value.Create(variable.Shape, result, NetUtil.CurrentDevice, true);
        }

        /// <summary>
        /// Get a batch from the given variable.
        /// </summary>
        /// <param name="variable">The variable to use.</param>
        /// <param name="source">The variable data.</param>
        /// <param name="indices">The array of data indices to use.</param>
        /// <param name="begin">The first index to use.</param>
        /// <param name="end">The last index to use.</param>
        /// <returns>A batch of values taken from the given variable.</returns>
        public static CNTK.Value GetBatch(
            this CNTK.Variable variable,
            float[] source,
            int[] indices,
            int begin,
            int end)
        {
            var num_indices = end - begin;
            var row_length = variable.Shape.TotalSize;
            var result = new float[num_indices];
            var row_index = 0;
            for (var index = begin; index != end; index++)
            {
                result[row_index++] = source[indices[index]];
            }
            return CNTK.Value.CreateBatch(variable.Shape, result, NetUtil.CurrentDevice, true);
        }

        /// <summary>
        /// Get a batch from the given variable.
        /// </summary>
        /// <param name="variable">The variable to use.</param>
        /// <param name="source">The variable data.</param>
        /// <param name="begin">The index of the first value to use.</param>
        /// <param name="end">The index of the last value to use.</param>
        /// <returns>A batch of values taken from the given variable.</returns>
        public static CNTK.Value GetBatch(
            this CNTK.Variable variable,
            float[][] source,
            int begin,
            int end)
        {
            var num_indices = end - begin;
            var result = new CNTK.NDArrayView[num_indices];
            var row_index = 0;
            for (var index = begin; index != end; index++)
            {
                var dataBuffer = source[index];
                var ndArrayView = new CNTK.NDArrayView(variable.Shape, dataBuffer, CNTK.DeviceDescriptor.CPUDevice, true);
                result[row_index++] = ndArrayView;
            }
            return CNTK.Value.Create(variable.Shape, result, NetUtil.CurrentDevice, true);
        }

        /// <summary>
        /// Get a batch from the given variable.
        /// </summary>
        /// <param name="variable">The variable to use.</param>
        /// <param name="source">The variable data.</param>
        /// <param name="begin">The index of the first value to use.</param>
        /// <param name="end">The index of the last value to use.</param>
        /// <returns>A batch of values taken from the given variable.</returns>
        public static CNTK.Value GetBatch(
            this CNTK.Variable variable,
            float[] source,
            int begin,
            int end)
        {
            var result = new float[end - begin];
            Array.Copy(source, begin, result, 0, result.Length);
            return CNTK.Value.CreateBatch(variable.Shape, result, NetUtil.CurrentDevice, true);
        }
    }
}
