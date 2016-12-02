using System;
using System.Data;
using System.IO;
using System.Text;

namespace ImportFromSql
{
    public class ImportService
    {
        public void ImportData(Stream outStream, IDataReader reader, OutputFormat format, Encoding enc, Func<int, string> getTableName)
        {
            if (outStream == null) throw new ArgumentNullException(nameof(outStream));
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (getTableName == null) throw new ArgumentNullException(nameof(getTableName));

            int idx = 0;
            string prevTableName = "";
            var formatter = OutputFormatter.Create(format, outStream, enc);
            do
            {
                var tableName = getTableName(idx);
                if (tableName == prevTableName)
                    tableName = "Таблица " + idx;
                WriteTable(tableName, reader, formatter);
                prevTableName = tableName;
                idx++;
            } while (reader.NextResult());
            formatter.Close();
        }

        public void WriteTable(string tableName, IDataReader reader, OutputFormatter formatter)
        {
            var values = new object[reader.FieldCount];
            formatter.NewTable(tableName, reader);
            while (reader.Read())
            {
                reader.GetValues(values);
                formatter.Write(values);
            }
        }
    }
}