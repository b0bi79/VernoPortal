using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Verno.Reports.Web
{
    public class DbLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new DbLogger();

        public void Dispose() { }

        private class DbLogger : ILogger
        {
            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                //File.AppendAllText(@"C:\temp\log.txt", formatter(state, exception));
                //Console.WriteLine(formatter(state, exception));
                Debug.WriteLine(formatter(state, exception));
            }

            public IDisposable BeginScope<TState>(TState state) => null;
        }
    }
}