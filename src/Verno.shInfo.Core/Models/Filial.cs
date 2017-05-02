using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Verno.ShInfo.Models
{
    [Table("Filialy")]
    public class Filial: Entity<int>
    {
        public string FilialNm { get; set; }
        public string fNm { get; set; }

        public IEnumerable<Proekt> Proekts { get; set; }
    }
}