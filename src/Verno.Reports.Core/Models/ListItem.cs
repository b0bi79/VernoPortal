using Abp.Domain.Entities;

namespace Verno.Reports.Models
{
    public class ListItem: Entity<string>
    {
        public string Name { get; set; }

        public ListItem(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}