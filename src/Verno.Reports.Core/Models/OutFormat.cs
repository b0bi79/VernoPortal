using Abp.Domain.Entities;

namespace Verno.Reports.Models
{
    public class OutFormat: Entity<string>
    {
        public OutFormat()
        {
        }

        public OutFormat(string id, string displayText, string generateUtil, string mimeType, string arguments, string description)
        {
            Id = id;
            Arguments = arguments;
            Description = description;
            DisplayText = displayText;
            GenerateUtil = generateUtil;
            MimeType = mimeType;
        }

        public string DisplayText { get; set; }
        public string GenerateUtil { get; set; }
        public string Arguments { get; set; }
        public string MimeType { get; set; }
        public string Description { get; set; }
    }
}