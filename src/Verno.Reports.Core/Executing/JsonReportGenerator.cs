using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Verno.Reports.DataSource;
using Verno.Reports.Models;

namespace Verno.Reports.Executing
{
    public class JsonReportGenerator : ReportGenerator
    {
        public JsonReportGenerator(ReportOutFormat format, IReportDataSourceFactory factory)
            : base(format, factory)
        {
        }


        protected override void WriteReport(Report report, IReportDataSource ds, Stream stream, Encoding encoding)
        {
            JsonWriter writer = new JsonTextWriter(new StreamWriter(stream, encoding));
            WriteReport(writer, ds.ExecuteReader(), report);
            writer.Flush();
        }

        public virtual void WriteReport(JsonWriter writer, IDataReader dbreader, Report report)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("id"); writer.WriteValue(report.Id);
            writer.WritePropertyName("name"); writer.WriteValue(report.Name);
            writer.WritePropertyName("category"); writer.WriteValue(report.Category);
            writer.WritePropertyName("description"); writer.WriteValue(report.Description);

            writer.WritePropertyName("parameters");
            WriteReportParameters(writer, report);

            writer.WritePropertyName("tables");
            WriteTables(writer, dbreader, report);

            writer.WriteEndObject();
        }

        public void WriteTables(JsonWriter writer, IDataReader dbreader, Report report)
        {
            writer.WriteStartArray();
            int idx = 0;
            do
            {
                var tableName = report.GetTableName(idx);
                writer.WriteStartObject();
                writer.WritePropertyName("name");
                writer.WriteValue(tableName);
                writer.WritePropertyName("rows");
                WriteDataTable(writer, dbreader);
                writer.WriteEnd();
                idx++;
            } while (dbreader.NextResult());

            writer.WriteEnd();
        }

        private static void WriteDataTable(JsonWriter writer, IDataReader dbreader)
        {
            writer.WriteStartArray();
            var fields = new string[dbreader.FieldCount];
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i] = dbreader.GetName(i);
            }
            while (dbreader.Read())
            {
                writer.WriteStartObject();
                for (int i = 0; i < fields.Length; i++)
                {
                    writer.WritePropertyName(fields[i]);
                    var value = dbreader[i];
                    if (value is DBNull)
                        writer.WriteNull();
                    else
                        writer.WriteValue(value);
                }
                writer.WriteEnd();
            }
        }

        private void WriteReportParameters(JsonWriter writer, Report report)
        {
            writer.WriteStartArray();
            foreach (var par in report.Parameters)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("name"); writer.WriteValue(par.Name);
                writer.WritePropertyName("type"); writer.WriteValue(par.ValueType);
                writer.WritePropertyName("displayText"); writer.WriteValue(par.DisplayText);
                if (par.ValueType.StartsWith("period"))
                {
                    var period = (Period)par.GetTypedValue();
                    writer.WritePropertyName("from"); writer.WriteValue(period.Date1);
                    writer.WritePropertyName("to"); writer.WriteValue(period.Date2);
                }
                else
                {
                    writer.WritePropertyName("value"); writer.WriteValue(par.Value);
                    writer.WritePropertyName("displayValue"); writer.WriteValue(GetDisplayValue(par, report));
                }
                writer.WriteEndObject();
            }
            writer.WriteEnd();
        }

        private string GetDisplayValue(RepParameter par, Report report)
        {
            if (par.Value == null)
                return null;

            var valuesList = ListValues.Parse(par, report);
            if (valuesList == null)
                return par.Value;

            if (par.DisplayType == DisplayType.ListBox)
            {
                var result = new StringBuilder();
                var items = (from istr in par.Value.Split(',', ' ')
                             select istr).ToArray();
                foreach (var item in items)
                {
                    var name = valuesList.GetName(item) ?? $"[{item}]";
                    if (result.Length > 0)
                        result.Append(", ");
                    result.Append(name);
                }
                return result.ToString();
            }
            else
            {
                var id = par.GetTypedValue();
                return valuesList.GetName(id);
            }
        }
    }
}