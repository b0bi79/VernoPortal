using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using Verno.Reports.DataSource;
using Verno.Reports.Models;

namespace Verno.Reports.Executing
{
    class XslTransformReportGenerator : ReportGenerator
    {
        public XslTransformReportGenerator(ReportOutFormat format, IReportDataSourceFactory factory)
            : base(format, factory)
        {
        }

        protected string ScriptsFolder { get; set; }

        protected override void WriteReport(Report report, IReportDataSource ds, Stream stream, Encoding encoding)
        {
            using (var xmlStream = new MemoryStream())
            {
                using (XmlWriter writer = new XmlTextWriter(xmlStream, encoding))
                {
                    WriteXml(writer, ds.ExecuteReader(), report, report.GetTableName);
                }
                xmlStream.Position = 0;

                using (var xmlr = new XmlTextReader(xmlStream))
                {
                    xmlr.Read();
                    while (xmlr.ReadState != ReadState.EndOfFile)
                    {
                        var myXslTrans = new XslCompiledTransform();
                        if (Format.Template == null)
                            throw new ArgumentException("Xsl template not specified.");
                        var templatePath = Path.Combine(ScriptsFolder, Format.Template);
                        myXslTrans.Load(templatePath);
                        using (var writer = new StreamWriter(stream, encoding))
                            myXslTrans.Transform(xmlr, null, writer);
                    }
                }
            }
        }

        public virtual void WriteXml(XmlWriter writer, IDataReader dbreader, Report report, Func<int, string> getTableName)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("Report");
            writer.WriteAttributeString("Id", report.Id.ToString());
            writer.WriteAttributeString("Name", report.Name);
            writer.WriteAttributeString("Category", report.Category);
            writer.WriteAttributeString("Description", report.Description);

            WriteReportParameters(writer, report);
            string tableName = report.Name;
            int idx = 0;
            string prevTableName = "";
            do
            {
                if (getTableName != null)
                    tableName = getTableName(idx);
                if (tableName == prevTableName)
                    tableName = "Таблица " + idx;
                WriteDataTable(tableName, writer, dbreader);
                prevTableName = tableName;
                idx++;
            } while (dbreader.NextResult());

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        private static void WriteDataTable(string tableName, XmlWriter writer, IDataReader dbreader)
        {
            writer.WriteStartElement("Table");
            writer.WriteAttributeString("Name", tableName);
            var fields = new string[dbreader.FieldCount];
            for (int i = 0; i < fields.Length; i++)
            {
                fields[i] = dbreader.GetName(i);
            }
            while (dbreader.Read())
            {
                writer.WriteStartElement("Row");
                for (int i = 0; i < fields.Length; i++)
                {
                    writer.WriteStartElement(fields[i]);
                    var value = dbreader[i];
                    if (value is DBNull)
                        writer.WriteAttributeString("xsi", "nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
                    else
                        writer.WriteValue(value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void WriteReportParameters(XmlWriter writer, Report report)
        {
            writer.WriteStartElement("Parameters");
            foreach (var par in report.Parameters)
            {
                writer.WriteStartElement(par.Name);
                writer.WriteAttributeString("DisplayText", par.DisplayText);
                if (par.ValueType.StartsWith("period"))
                {
                    var period = (Period)par.GetTypedValue();
                    writer.WriteStartElement("From");
                    writer.WriteValue(period.Date1);
                    writer.WriteEndElement();
                    writer.WriteStartElement("To");
                    writer.WriteValue(period.Date2);
                    writer.WriteEndElement();
                }
                else
                {
                    writer.WriteAttributeString("Value", par.Value);
                    writer.WriteValue(GetDisplayValue(par, report));
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
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