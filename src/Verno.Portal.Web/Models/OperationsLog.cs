using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Verno.Portal.Web.Models
{
    public class OperationsLog : Entity<int>
    {
        public OperationsLog()
        {
        }

        public OperationsLog(string operation, string message = null, int? userId = null)
        {
            Date = DateTime.Now;
            Message = message;
            Operation = operation;
            UserId = userId;
        }

        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
        public int? UserId { get; set; }
        public string Operation { get; set; }
        public string Message { get; set; }
    }
}