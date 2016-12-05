using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Verno.Portal.Web.Modules.Returns
{
    [AutoMap(typeof(ReturnData))]
    public class ReturnDto : EntityDto<int>
    {
        public int ShopNum { get; set; }
        public DateTime DocDate { get; set; }
        public string DocNum { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public decimal Summ { get; set; }
        public int Liniah { get; set; }
        public string LiniahTip { get; set; }
        public int? ReturnId { get; set; }
        public ReturnStatus Status { get; set; }
    }

    [AutoMap(typeof(ReturnFile))]
    public class ReturnFileDto : EntityDto<int>
    {
        public ReturnFileDto()
        {
        }

        public ReturnFileDto(int id, string error, string fileName, long fileSize, string name, int returnId, string url) : base(id)
        {
            Error = error;
            FileName = fileName;
            FileSize = fileSize;
            Name = name;
            ReturnId = returnId;
            Url = url;
        }

        public int ReturnId { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string Error { get; set; }
        public string Url { get; set; }
    }

    /*public class FileUploadInput : EntityDto<int>
    {
        public int Rasxod { get; set; }
        public FormFile File { get; set; }
    }*/
}