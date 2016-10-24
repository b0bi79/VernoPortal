using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
//using bb.Kit.Serializing;

namespace ImportFromSql
{
    public class XmlOutputFormatter : OutputFormatter
    {
        private readonly XmlTextWriter _writer;
        private bool _tableStarted;
        string[] _columnList;

        public XmlOutputFormatter(Stream stream, Encoding encoding)
        {
            _writer = new XmlTextWriter(stream, encoding);
            _writer.WriteStartElement("Tables");
        }

        public override void Write(object[] row)
        {
            _writer.WriteStartElement("Row");
            for (int i = 0; i < _columnList.Length; i++)
            {
                var fld = _columnList[i];
                _writer.WriteAttributeString(fld, row[i].ToString());
            }
            _writer.WriteEndElement();
        }

        public override void Close()
        {
            _writer.Close();
        }

        public override void NewTable(string name, IDataReader reader)
        {
            if (_tableStarted)
                _writer.WriteEndElement();
            _writer.WriteStartElement(name);
            _tableStarted = true;

            _columnList = BuildFieldDescriptions(reader);
        }

        private string[] BuildFieldDescriptions(IDataReader sqlReader)
        {
            var fields = new List<string>(sqlReader.FieldCount);
            for (int i = 0; i < sqlReader.FieldCount; i++)
            {
                fields.Add(sqlReader.GetName(i));
            }
            return fields.ToArray();
        }
    }
}