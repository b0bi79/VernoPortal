using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.AutoMapper;
using Abp.Domain.Entities;

namespace Verno.ShInfo.Models.Dto
{
    [AutoMap(typeof(Filial))]
    public class FilialDto
    {
        public int Id { get; set; }
        public string FilialNm { get; set; }
        public string fNm { get; set; }
    }
}