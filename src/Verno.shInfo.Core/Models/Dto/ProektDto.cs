using System.ComponentModel.DataAnnotations.Schema;
using Abp.AutoMapper;
using Abp.Domain.Entities;

namespace Verno.ShInfo.Models.Dto
{
    [AutoMap(typeof(Proekt))]
    public class ProektDto
    {
        public int Id { get; set; }
        public int FilialId { get; set; }
        public int GlProekt { get; set; }
        public string ImahProekta { get; set; }
        public string ImahPr { get; set; }
        public short Zakryt { get; set; }

        public FilialDto Filial { get; set; }
    }

}