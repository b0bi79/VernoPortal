using System;
using System.Data;
using Verno.Reports.Models;

namespace Verno.Reports.DataSource
{
    public abstract class ReportDataSource : IDisposable
    {
        private bool _disposed;
        protected Report Report;

        protected ReportDataSource(Report report)
        {
            Report = report;
        }

        public IDbConnection Connection { get; private set; }

        public static ReportDataSource ReadReportData(Report report)
        {
            if (report.SqlFile!=null)
            {
                var source = new SqlFileReportDataSource(report);
                source.OpenConnection();
                return source;
            }
            else
            {
                var source = new SqlProcReportDataSource(report);
                source.OpenConnection();
                return source;
            }
        }

        public IDbConnection OpenConnection()
        {
            if (Connection != null)
                throw new InvalidOperationException("OpenConnection already calls.");

            Connection = Report.Connection.CreateConnection();
            Connection.Open();
            return Connection;
        }

        public void Close()
        {
            Connection?.Close();
        }

        public abstract IDataReader ExecuteReader();

        private static bool IsMultivalue(RepParameter parameter)
        {
            return parameter.DisplayType == DisplayType.ListBox;
        }

        protected static object GetParameterValue(RepParameter par)
        {
            if (par.Value == null)
                return DBNull.Value;
            return IsMultivalue(par)
                ? par.Value
                : par.GetTypedValue(); //ParameterValueConverter.ConvertTo(par.Value, par.ValueType);
        }

        #region Disposing

        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Close();
                    // Free other state (managed objects).
                    if (Connection != null)
                    {
                        Connection.Dispose();
                        Connection = null;
                    }
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                _disposed = true;
            }
        }

        // Use C# destructor syntax for finalization code.
        ~ReportDataSource()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }

        #endregion
    }
}