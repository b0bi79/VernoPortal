using System;
using System.Data;
using System.IO;
using System.Text;

namespace ImportFromSql
{
    public abstract class OutputFormatter
    {
        public static OutputFormatter Create(OutputFormat format, Stream stream, Encoding encoding)
        {
            switch (format)
            {
                case OutputFormat.XLS:
                    return new XlsOutputFormatter(stream);
                case OutputFormat.XML:
                    return new XmlOutputFormatter(stream, encoding);
                default:
                    throw new NotImplementedException("Formatter is not jet implemented.");
            }
        }

        public abstract void Write(object[] row);
        public abstract void Close();
        public abstract void NewTable(string name, IDataReader reader);
    }

    public enum OutputFormat
    {
        XLS,
        CSV,
        XML
    }
}