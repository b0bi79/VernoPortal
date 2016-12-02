using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Verno.Reports.Models
{
    [Table("ReportParams")]
    public class RepParameter: Entity<int>
    {
        public RepParameter()
        {
        }

        public RepParameter(string name, string displayText, TypeCode valueType, string displayType, bool isRequired = true, string valueFormat = null,
            string value = null, string listValues = null, string helpText = null)
            : this(name, displayText, valueType.ToString(), displayType, isRequired, valueFormat, value, listValues, helpText)
        {
        }
        public RepParameter(string name, string displayText, string valueType, string displayType, bool isRequired = true, string valueFormat = null, string value = null, string listValues = null, string helpText = null)
        {
            DisplayText = displayText;
            DisplayType = displayType;
            HelpText = helpText;
            IsRequired = isRequired;
            ListValues = listValues;
            Name = name;
            Value = value;
            ValueFormat = valueFormat;
            ValueType = valueType;
        }

        public string Name { get; set; }
        public string DisplayText { get; set; }
        public string ValueType { get; set; }
        public string DisplayType { get; set; }
        public bool IsRequired { get; set; }
        public string ValueFormat { get; set; }
        [Column("DefValue")]
        public string Value { get; set; }
        public string ListValues { get; set; }
        public string HelpText { get; set; }

        public int ReportId { get; set; }
        public Report Report { get; set; }

        public object GetTypedValue()
        {
            return Value.ChangeType(TypeExtensions.ParseTypeCode(ValueType).ToType());
        }
    }
}