// Copyright (c) Microsoft Corporation. 
//  Licensed under the MIT License.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Azure.AI.Language.MCP.Common.Logger
{
    /// <summary>
    /// Extension methods for adding a simple file logger to the logging builder.
    /// </summary>
    public static class FileLoggerExtensions
    {
        /// <summary>
        /// Adds a simple file logger to the logging builder.
        /// </summary>
        /// <param name="builder">The logging builder to add the file logger to.</param>
        /// <param name="filePath">The path to the log file.</param>
        /// <param name="maxSize">The maximum size (in bytes) for a single log file.</param>
        /// <param name="maxFileCount">The maximum number of log files to retain.</param>
        /// <returns>The logging builder for chaining.</returns>
        public static ILoggingBuilder AddSimpleFileLogger(this ILoggingBuilder builder, string filePath, long maxSize, int maxFileCount)
        {
            builder.Services.AddSingleton<ILoggerProvider>(new FileLoggerProvider(filePath, maxSize, maxFileCount));
            return builder;
        }
    }
}
