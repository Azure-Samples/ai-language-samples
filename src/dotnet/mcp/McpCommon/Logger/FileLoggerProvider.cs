// Copyright (c) Microsoft Corporation. 
//  Licensed under the MIT License.

using Microsoft.Extensions.Logging;

namespace Azure.AI.Language.MCP.Common.Logger
{
    /// <summary>
    /// Provides a logger that writes log messages to a file with support for file size and file count limits.
    /// </summary>
    public class FileLoggerProvider : ILoggerProvider
    {
        /// <summary>
        /// The path to the log file.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// The maximum size of a single log file in bytes.
        /// </summary>
        private readonly long _maxSize;

        /// <summary>
        /// The maximum number of log files to retain.
        /// </summary>
        private readonly int _maxFileCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLoggerProvider"/> class.
        /// </summary>
        /// <param name="filePath">The path to the log file.</param>
        /// <param name="maxSize">The maximum size of a single log file in bytes. Defaults to 10 MB if not specified.</param>
        /// <param name="maxFileCount">The maximum number of log files to retain. Defaults to 5 if not specified.</param>
        public FileLoggerProvider(string filePath, long? maxSize, int? maxFileCount)
        {
            _filePath = filePath;
            _maxSize = maxSize ?? 10 * 1024 * 1024; // Default to 10 MB
            _maxFileCount = maxFileCount ?? 5; // Default to 5 log files
        }

        /// <summary>
        /// Creates a new <see cref="FileLogger"/> instance for the specified category.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>A <see cref="FileLogger"/> instance.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(categoryName, _filePath, _maxSize, _maxFileCount);
        }

        public void Dispose() { }
    }
}
