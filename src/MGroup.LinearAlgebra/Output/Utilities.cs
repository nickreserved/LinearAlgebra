﻿using System;
using System.IO;

namespace MGroup.LinearAlgebra.Output
{
    /// <summary>
    /// Write Values to a stream using the provided <see cref="StreamWriter"/>.
    /// </summary>
    /// <param name="writer">The writer with which to write Values to the stream.</param>
    internal delegate void WriteToStream(StreamWriter writer);

    /// <summary>
    /// Provides utility methods that handle writing Values to various streams. The caller only needs to specify the Values that will
    /// be written.
    /// Authors: Serafeim Bakalakos
    /// </summary>
    internal static class Utilities
    {
        internal static void WriteToConsole(WriteToStream callback)
        {
            // Point the stream to standard output
            var writer = new StreamWriter(Console.OpenStandardOutput());
            writer.AutoFlush = true;
            Console.SetOut(writer);

            // Call the abstract method
            callback(writer);

            // Recover the standard output stream
            var standardOutput = new StreamWriter(Console.OpenStandardOutput());
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
        }

        internal static void WriteToFile(WriteToStream callback, string path, bool append = false)
        {
            using (var writer = new StreamWriter(path, append))
            {
#if DEBUG
                writer.AutoFlush = true; // To look at intermediate output at certain breakpoints
#endif
                callback(writer);
            }
        }
    }
}
