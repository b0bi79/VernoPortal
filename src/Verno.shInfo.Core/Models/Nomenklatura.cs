using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;

namespace Verno.ShInfo.Models
{
    public class Nomenklatura : Entity<int>
    {
        // PRIMARY KEY VidTovara 
        public string Naimenovanie { get; set; }
        public decimal KolichestvoUpakovka { get; set; }
        public string EdinicyIzmereniah { get; set; }
    }
}