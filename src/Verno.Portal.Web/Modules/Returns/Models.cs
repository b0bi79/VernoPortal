using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Verno.Portal.Web.Modules.Returns
{
    [Table("vReturns")]
    public class ReturnData : Entity<int>
    {
        // PRIMARY KEY Rasxod
        public int ShopNum { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DocDate { get; set; }
        public string DocNum { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public decimal Summ { get; set; }
        public short Liniah { get; set; }
        public string LiniahTip { get; set; }
        public int? ReturnId { get; set; }
        public int Status { get; set; }

        //public Return Return { get; set; }
    }


    [Table("Returns")]
    public class Return : Entity<int>
    {
        public Return()
        {
        }

        public Return(DateTime docDate, string docNum, int shopNum, int supplierId)
        {
            DocDate = docDate;
            DocNum = docNum;
            ShopNum = shopNum;
            SupplierId = supplierId;
        }

        public int ShopNum { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DocDate { get; set; }
        public string DocNum { get; set; }
        public int SupplierId { get; set; }

        public ReturnStatus Status { get; set; }

        public virtual ICollection<ReturnFile> Files { get; set; } =  new List<ReturnFile>();

        public ReturnFile AddFile(string name, string fileName, string savedName)
        {
            var file = new ReturnFile()
            {
                Name = name,
                FileName = fileName,
                SavedName = savedName,
                DateLot = DateTime.Now,
                Deleted = false,
                LastModificationTime = DateTime.Now,
            };
            Files.Add(file);
            return file;
        }
    }

    [Table("ReturnFiles")]
    public class ReturnFile : Entity<int>, IModificationAudited
    {
        public int ReturnId { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string SavedName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateLot { get; set; }
        public bool Deleted { get; set; }
        [Column("EditDate", TypeName = "datetime")]
        public DateTime? LastModificationTime { get; set; }
        [Column("EditUserId")]
        public long? LastModifierUserId { get; set; }

        [ForeignKey("ReturnId")]
        public Return Return { get; set; }
    }

    public enum ReturnStatus
    {
        None = 0,
        Processed = 10,
    }
}