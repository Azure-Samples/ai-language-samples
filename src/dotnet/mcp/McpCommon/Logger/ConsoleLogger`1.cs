// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Logging;

namespace Azure.AI.Language.MCP.Common.Logger
{
    /// <summary>
    /// A no-operation logger implementation of <see cref="ILogger"/> that writes log messages to the console.
    /// </summary>
    public class ConsoleLogger<TType> : ILogger<TType>
    {
        /// <summary>
        /// Begins a logical operation scope. This implementation does not support scopes and returns a no-op disposable.
        /// </summary>
        /// <typeparam name="TState">The type of the state to associate with the scope.</typeparam>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>A disposable object that ends the scope on disposal.</returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            // No scope support, return a no-op disposable
            return new NoOpDisposable();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // Always enabled
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var typeName = typeof(TType).Name;
            var message = formatter(state, exception);
            var output = $"[{logLevel}] {eventId.Id} [{typeName}]: {message}";
            if (exception != null)
            {
                output += $" Exception: {exception}";
            }

            Console.WriteLine(output);
        }

        private class NoOpDisposable : IDisposable
        {
            public void Dispose() { }
        }
    }
}