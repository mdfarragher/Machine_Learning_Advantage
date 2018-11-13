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
            var size = Marshal.SizeOf(typeof(T));
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

    }
}
