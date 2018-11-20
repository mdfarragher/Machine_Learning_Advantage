using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Pensar
{
    /// <summary>
    /// A collection of utilities to work with data files.
    /// </summary>
    public static class DataUtil
    {
        /// <summary>
        /// Unzip the given archive to the specified destination path.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="destinationPath"></param>
        public static void Unzip(string filepath, string destinationPath)
        {
            System.IO.Compression.ZipFile.ExtractToDirectory(filepath, destinationPath);
        }

        /// <summary>
        /// Load the given binary file from disk.
        /// </summary>
        /// <param name="filepath">The filename of the file to load.</param>
        /// <param name="numRows">The number of rows to load.</param>
        /// <param name="numColumns">The number of columns to load.</param>
        /// <returns></returns>
        public static T[][] LoadBinary<T>(
            string filepath,
            int numRows,
            int numColumns)
        {
            var size = Marshal.SizeOf(typeof(T)); // warning: unreliable for char!
            var buffer = new byte[size * numRows * numColumns];
            using (var reader = new System.IO.BinaryReader(System.IO.File.OpenRead(filepath)))
            {
                reader.Read(buffer, 0, buffer.Length);
            }
            var dst = new T[numRows][];
            for (int row = 0; row < dst.Length; row++)
            {
                dst[row] = new T[numColumns];
                Buffer.BlockCopy(buffer, row * numColumns * size, dst[row], 0, numColumns * size);
            }
            return dst;
        }

        /// <summary>
        /// Load the given binary file from disk.
        /// </summary>
        /// <param name="filepath">The filename of the file to load.</param>
        /// <param name="numRows">The number of rows to load.</param>
        /// <returns></returns>
        public static T[] LoadBinary<T>(
            string filepath,
            int numRows)
        {
            var size = Marshal.SizeOf(typeof(T));
            var buffer = new byte[size * numRows];
            using (var reader = new System.IO.BinaryReader(System.IO.File.OpenRead(filepath)))
            {
                reader.Read(buffer, 0, buffer.Length);
            }
            var dst = new T[numRows];
            System.Buffer.BlockCopy(buffer, 0, dst, 0, buffer.Length);
            return dst;
        }

        /// <summary>
        /// Get an image reader to sequentially read images from disk for training.
        /// </summary>
        /// <param name="mapFilePath">The path to the map file</param>
        /// <param name="imageWidth">The width to scale all images to</param>
        /// <param name="imageHeight">The height to scale all images to</param>
        /// <param name="numChannels">The number of channels to transform all images to</param>
        /// <param name="numClasses">The number of label classes in this training set</param>
        /// <param name="augmentData">Set to true to use data augmentation to expand the training set</param>
        /// <returns></returns>
        public static CNTK.MinibatchSource GetImageReader(string mapFilePath, int imageWidth, int imageHeight, int numChannels, int numClasses, bool augmentData)
        {
            var transforms = new List<CNTK.CNTKDictionary>();
            if (augmentData)
            {
                var randomSideTransform = CNTK.CNTKLib.ReaderCrop("RandomSide",
                  new Tuple<int, int>(0, 0),
                  new Tuple<float, float>(0.8f, 1.0f),
                  new Tuple<float, float>(0.0f, 0.0f),
                  new Tuple<float, float>(1.0f, 1.0f),
                  "uniRatio");
                transforms.Add(randomSideTransform);
            }
            var scaleTransform = CNTK.CNTKLib.ReaderScale(imageWidth, imageHeight, numChannels);
            transforms.Add(scaleTransform);

            var imageDeserializer = CNTK.CNTKLib.ImageDeserializer(mapFilePath, "labels", (uint)numClasses, "features", transforms);
            var minibatchSourceConfig = new CNTK.MinibatchSourceConfig(new CNTK.DictionaryVector() { imageDeserializer });
            return CNTK.CNTKLib.CreateCompositeMinibatchSource(minibatchSourceConfig);
        }

    }
}
