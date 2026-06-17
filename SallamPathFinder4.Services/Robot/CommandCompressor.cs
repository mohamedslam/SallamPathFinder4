#region File Header
/// <summary>
/// File: CommandCompressor.cs
/// Description: Compresses robot commands for efficient storage
/// Author: Mohamed ElSayed Sallam
/// Date: 2026-04-14
/// </summary>
#endregion

#region Namespace Imports
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using SallamPathFinder4.Core.Models.Robot;
#endregion

namespace SallamPathFinder4.Services.Robot
{
    #region Class Documentation
    /// <summary>
    /// Compresses robot commands for efficient storage on SD card
    /// </summary>
    #endregion
    public sealed class CommandCompressor
    {
        #region Constants
        private const int MAX_COMMANDS_PER_FILE = 100000;
        #endregion

        #region Public Methods - Compression
        /// <summary>
        /// Compresses commands to binary format
        /// </summary>
        public byte[] CompressToBinary(List<RobotCommand> commands)
        {
            using (var ms = new MemoryStream())
            {
                // Write header
                byte[] header = Encoding.ASCII.GetBytes("SALL");
                ms.Write(header, 0, header.Length);

                // Write version
                ms.WriteByte(1);

                // Write command count
                byte[] countBytes = BitConverter.GetBytes(commands.Count);
                ms.Write(countBytes, 0, countBytes.Length);

                // Write each command
                foreach (var command in commands)
                {
                    byte[] cmdBytes = command.ToBinary();
                    ms.Write(cmdBytes, 0, cmdBytes.Length);
                }

                // Compress using GZip
                byte[] uncompressed = ms.ToArray();
                return CompressGZip(uncompressed);
            }
        }

        /// <summary>
        /// Decompresses binary format to commands
        /// </summary>
        public List<RobotCommand> DecompressFromBinary(byte[] compressedData)
        {
            var result = new List<RobotCommand>();

            // Decompress
            byte[] data = DecompressGZip(compressedData);

            using (var ms = new MemoryStream(data))
            {
                // Read header
                byte[] header = new byte[4];
                ms.Read(header, 0, 4);
                string headerStr = Encoding.ASCII.GetString(header);

                if (headerStr != "SALL")
                {
                    throw new InvalidDataException("Invalid file format");
                }

                // Read version
                int version = ms.ReadByte();

                // Read command count
                byte[] countBytes = new byte[4];
                ms.Read(countBytes, 0, 4);
                int commandCount = BitConverter.ToInt32(countBytes, 0);

                // Read commands (simplified - actual parsing would need command format)
                for (int i = 0; i < commandCount; i++)
                {
                    // This would need proper command parsing
                    // For now, return empty list
                }
            }

            return result;
        }

        /// <summary>
        /// Exports commands to text file for debugging
        /// </summary>
        public void ExportToTextFile(List<RobotCommand> commands, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("# SallamPathFinder4 - Robot Commands");
                writer.WriteLine($"# Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine($"# Total Commands: {commands.Count}");
                writer.WriteLine("#");

                int step = 1;
                foreach (var command in commands)
                {
                    writer.WriteLine($"{step,4}: {command}");
                    step++;
                }
            }
        }

        /// <summary>
        /// Calculates estimated storage size
        /// </summary>
        public long EstimateStorageSize(List<RobotCommand> commands)
        {
            long totalBytes = 0;

            foreach (var command in commands)
            {
                totalBytes += command.ToBinary().Length;
            }

            // Add header overhead
            totalBytes += 16;

            // Compress (estimate 50% compression for text, 20% for binary)
            totalBytes = (long)(totalBytes * 0.8);

            return totalBytes;
        }
        #endregion

        #region Private Methods
        private byte[] CompressGZip(byte[] data)
        {
            using (var output = new MemoryStream())
            {
                using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
                {
                    gzip.Write(data, 0, data.Length);
                }
                return output.ToArray();
            }
        }

        private byte[] DecompressGZip(byte[] compressedData)
        {
            using (var input = new MemoryStream(compressedData))
            using (var gzip = new GZipStream(input, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                gzip.CopyTo(output);
                return output.ToArray();
            }
        }
        #endregion
    }
}