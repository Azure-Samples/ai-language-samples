// Copyright (c) Microsoft Corporation. 
//  Licensed under the MIT License.

using Microsoft.Extensions.Logging;

namespace Azure.AI.Language.MCP.Common.Logger
{
    /// <summary>
    /// Provides logging functionality to a file with support for file size-based rotation.
    /// </summary>
    internal class FileLogger : ILogger
    {
        private const string DefaultLogDirectory = "Logs";

        /// <summary>
        /// The path to the log file.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// The category name for the logger.
        /// </summary>
        private readonly string _categoryName;

        /// <summary>
        /// Maximum file size in bytes before rotation occurs.
        /// </summary>
        private readonly long _maxFileSizeBytes;

        /// <summary>
        /// Maximum number of rolling log files to keep.
        /// </summary>
        private readonly int _maxRollingFiles;

        /// <summary>
        /// Lock object for thread-safe file operations.
        /// </summary>
        private static readonly object _lock = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogger"/> class.
        /// </summary>
        /// <param name="categoryName">The category name for the logger.</param>
        /// <param name="filePath">The path to the log file.</param>
        /// <param name="maxFileSizeBytes">Maximum file size in bytes before rotation.</param>
        /// <param name="maxRollingFiles">Maximum number of rolling log files to keep.</param>
        public FileLogger(string categoryName, string filePath, long maxFileSizeBytes, int maxRollingFiles)
        {
            _categoryName = categoryName;

            var directory = Path.GetDirectoryName(filePath) ?? DefaultLogDirectory;

            _filePath = filePath;
            if (!Directory.Exists(directory))
            {
                _ = Directory.CreateDirectory(directory);
            }

            _maxFileSizeBytes = maxFileSizeBytes > 0 ? maxFileSizeBytes : 10 * 1024 * 1024; // Default to 10 MB if not set
            _maxRollingFiles = maxRollingFiles > 0 ? maxRollingFiles : 5; // Default to 5 rolling files if not set
        }

        /// <inheritdoc />
        public IDisposable? BeginScope<TState>(TState state) => null;

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel) => true;

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId,
            TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = $"{DateTime.Now:u} [{logLevel}] {_categoryName}: {formatter(state, exception)}{Environment.NewLine}";

            lock (_lock)
            {
                RotateIfNeeded();
                File.AppendAllText(_filePath, message);
            }
        }

        /// <summary>
        /// Rotates the log file if its size exceeds the configured maximum.
        /// </summary>
        private void RotateIfNeeded()
        {
            if (!File.Exists(_filePath))
            {
                return;
            }

            var fileInfo = new FileInfo(_filePath);
            if (fileInfo.Length < _maxFileSizeBytes)
            {
                return;
            }

            // Rotate files
            for (int i = _maxRollingFiles - 1; i >= 1; i--)
            {
                var src = $"{_filePath}.{i}";
                var dest = $"{_filePath}.{i + 1}";
                if (File.Exists(src))
                {
                    if (i + 1 > _maxRollingFiles)
                    {
                        File.Delete(src);
                    }
                    else
                    {
                        File.Move(src, dest, overwrite: true);
                    }
                }
            }

            File.Move(_filePath, $"{_filePath}.1", overwrite: true);
        }
    }
}
