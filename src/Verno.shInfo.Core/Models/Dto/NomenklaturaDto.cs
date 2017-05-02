using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Abp.Domain.Entities;

namespace Verno.ShInfo.Models.Dto
{
    [AutoMap(typeof(Nomenklatura))]
    public class NomenklaturaDto
    {
        public int Id { get; set; }
        public string Naimenovanie { get; set; }
        public decimal KolichestvoUpakovka { get; set; }
        public string EdinicyIzmereniah { get; set; }
    }
}