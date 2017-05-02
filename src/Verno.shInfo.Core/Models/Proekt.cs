using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Verno.ShInfo.Models
{
    [Table("Proekty")]
    public class Proekt: Entity<int>
    {
        [Column("Filial")]
        public int FilialId { get; set; }
        public int GlProekt { get; set; }
        public string ImahProekta { get; set; }
        public string ImahPr { get; set; }
        public short Zakryt { get; set; }

        [ForeignKey("FilialId")]
        public Filial Filial { get; set; }
    }

}