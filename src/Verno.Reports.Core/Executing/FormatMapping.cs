using System;
using System.Collections.Generic;
using ImportFromSql;
using Verno.Reports.Models;

namespace Verno.Reports.Executing
{
    public class FormatMapping
    {
        private static readonly Dictionary<string, FormatMapping> Bindings = new Dictionary<string, FormatMapping>
        {
            {"XLS", new FormatMapping(OutputFormat.XLS, ".xls")},
            {"HTML", new FormatMapping(OutputFormat.XML, ".html")},
            {"PDF", new FormatMapping(OutputFormat.XML, ".pdf")},
            {"CSV", new FormatMapping(OutputFormat.CSV, ".csv")},
        };

        public static FormatMapping GetOutputFormat(OutFormat format)
        {
            if (format == null) throw new ArgumentNullException(nameof(format));
            FormatMapping mapping;
            if (Bindings.TryGetValue(format.Arguments, out mapping))
                return mapping;

            return new FormatMapping(OutputFormat.XML, '.' + format.Arguments);
        }

        private FormatMapping(OutputFormat outputFormat, string fileExtension)
        {
            OutputFormat = outputFormat;
            FileExtension = fileExtension;
        }

        public OutputFormat OutputFormat { get; private set; }
        public string FileExtension { get; private set; }
    }
}