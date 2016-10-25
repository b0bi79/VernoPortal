﻿using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Verno.Reports.Models;

namespace Verno.Reports.Reports.Dtos
{
    [AutoMapFrom(typeof(OutFormat))]
    public class OutFormatDto : EntityDto<string>
    {
        public string DisplayText { get; set; }
    }
}