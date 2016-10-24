using System;
using System.Data;
using System.IO;
using System.Text;

namespace ImportFromSql
{
    public class ImportService
    {
        public void ImportData(IDataReader reader, string outputFile, OutputFormat format, Encoding enc, Func<int, string> getTableName)
        {
            string tableName = Path.GetFileNameWithoutExtension(outputFile);

            using (var outStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                int idx = 0;
                string prevTableName = "";
                var formatter = OutputFormatter.Create(format, outStream, enc);
                do
                {
                    if (getTableName != null) 
                        tableName = getTableName(idx);
                    if (tableName == prevTableName)
                        tableName = "Таблица " + idx;
                    WriteTable(tableName, reader, formatter);
                    prevTableName = tableName;
                    idx++;
                } while (reader.NextResult());
                formatter.Close();
            }
        }

        public void WriteTable(string tableName, IDataReader reader, OutputFormatter formatter)
        {
            var values = new Object[reader.FieldCount];
            formatter.NewTable(tableName, reader);
            while (reader.Read())
            {
                reader.GetValues(values);
                formatter.Write(values);
            }
        }
    }
}