﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Linq;
using System.Net.Mime;
using Abp.Authorization;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Verno.Identity.Users;
using Verno.Reports.Web.ActionResults;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Verno.Configuration;
using Verno.Reports.Web.Utils;

namespace Verno.Reports.Web.Modules.Returns
{
    public interface IReturnsAppService
    {
        Task<ListResultDto<ReturnDto>> GetList(DateTime dfrom, DateTime dto, bool unreclaimedOnly);
        Task<ListResultDto<ReturnFileDto>> GetFilesList(int rasxod);
        Task<ActionResult> File(int fileId);
        //Task<ReturnFileDto> UploadFile(/*int rasxod, */IFormFile file);
        Task<ReturnFileDto> DeleteFile(int fileId);
    }

    [Route("api/services/app/returns")]
    [AbpAuthorize(ReturnsPermissionNames.Documents_Returns)]
    public class ReturnsAppService : ApplicationService, IReturnsAppService
    {
        private readonly ReturnsDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly HttpContext _httpContext;

        public ReturnsAppService(ReturnsDbContext context, IOptions<AppSettings> appSettings, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _httpContext = contextAccessor.HttpContext;
        }

        public UserManager UserManager { get; set; }

        #region Implementation of IReturnAppService

        [HttpGet]
        [Route("{dfrom:datetime}!{dto:datetime}")]
        public async Task<ListResultDto<ReturnDto>> GetList(DateTime dfrom, DateTime dto, bool unreclaimedOnly)
        {
            int shopNum = await GetUserShopNum();
            var result = from d in _context.ReturnDatas
                         where d.DocDate >= dfrom && d.DocDate <= dto &&
                               (d.ShopNum == shopNum || shopNum == 0)
                         select d;
            if (unreclaimedOnly)
                result = result.Where(r => r.Status == 0 /*ReturnStatus.None*/);
            return new ListResultDto<ReturnDto>((await result.Take(500).ToListAsync()).MapTo<List<ReturnDto>>());
        }

        [HttpGet]
        [Route("{rasxod}/files")]
        public async Task<ListResultDto<ReturnFileDto>> GetFilesList(int rasxod)
        {
            var fileEntities = await (from r in _context.ReturnFiles
                    where !r.Deleted && r.Return.Rasxod == rasxod
                    select r)
                .ToListAsync();
            var result = new List<ReturnFileDto>();
            foreach (var file in fileEntities)
            {
                var item = new ReturnFileDto(file.Id, "", file.FileName, 0, file.Name, file.ReturnId, 
                    _httpContext.CreateUrl("/api/services/app/returns/files/" + file.Id));
                var fileInfo = new FileInfo(Path.Combine(ServPath, file.SavedName));
                if (fileInfo.Exists)
                    item.FileSize = fileInfo.Length;
                else
                    item.Error = "Файл был удалён с сервера.";
                result.Add(item);
            }
            return new ListResultDto<ReturnFileDto>(result);
        }

        [HttpGet]
        [AbpAuthorize(ReturnsPermissionNames.Documents_Returns_GetFile)]
        [Route("files/{fileId}")]
        public async Task<ActionResult> File(int fileId)
        {
            var resultFile = await _context.ReturnFiles.FirstOrDefaultAsync(f => f.Id == fileId);
            if (resultFile != null)
            {
                var filepath = Path.Combine(ServPath, resultFile.SavedName);
                if (System.IO.File.Exists(filepath))
                {
                    string contentType;
                    new FileExtensionContentTypeProvider().TryGetContentType(resultFile.FileName, out contentType);
                    return new PhysicalFileResultWithContentDisposition(filepath, contentType ?? "application/octet-stream", new ContentDisposition()
                    {
                        Inline = true,
                        FileName = Path.GetFileName(resultFile.FileName)
                    });
                }
            }
            return new NotFoundResult();
        }

        [HttpDelete]
        [AbpAuthorize(ReturnsPermissionNames.Documents_Returns_DeleteFile)]
        [Route("files/{fileId}")]
        public async Task<ReturnFileDto> DeleteFile(int fileId)
        {
            var file = await _context.ReturnFiles.FirstOrDefaultAsync(x => x.Id == fileId);
            if (file==null)
                throw new ApplicationException($"Файла с id={fileId} не существует.");
            file.Deleted = true;
            var user = await GetCurrentUserAsync();
            file.EditUser = user.UserName;
            await _context.SaveChangesAsync(true);
            return file.MapTo<ReturnFileDto>();
        }

        /*[HttpPost]
        [Route("api/services/app/returns/{rasxod}/files")]
        [Route("api/services/app/returns/files")]*/
        internal async Task<ReturnFileDto> UploadFile(int rasxod, IFormFile file)
        {
            var fileName = ContentDispositionHeaderValue
                .Parse(file.ContentDisposition)
                .FileName
                .Trim('"');

            string savedName;
            string savedFilePath;
            do
            {
                savedName = Path.GetRandomFileName();
                savedFilePath = Path.Combine(ServPath, savedName);
            } while (System.IO.File.Exists(savedFilePath));

            using (FileStream fs = System.IO.File.Create(savedFilePath))
            {
                await file.CopyToAsync(fs);
                await fs.FlushAsync();
            }

            var user = await GetCurrentUserAsync();
            Return r = await _context.Returns.FirstOrDefaultAsync(x => x.Rasxod == rasxod);
            if (r == null)
            {
                r = new Return(rasxod);
                _context.Add(r);
                await _context.SaveChangesAsync(true); //To get new user's id.
            }
            var fileEntity = r.AddFile(Path.GetFileNameWithoutExtension(fileName), fileName, savedName);
            fileEntity.EditUser = user.UserName;
            _context.Add(fileEntity);

            await _context.SaveChangesAsync(true); //To get new user's id.

            var result = fileEntity.MapTo<ReturnFileDto>();
            result.FileSize = file.Length;
            result.Url = _httpContext.CreateUrl("/api/services/app/returns/files/" + fileEntity.Id);
            return result;
        }

        private string _servPath;
        public string ServPath => _servPath ?? (_servPath = _appSettings.PrintFilesPath);

        #endregion

        private async Task<int> GetUserShopNum()
        {
            var user = await GetCurrentUserAsync();
            var claims = await UserManager.GetClaimAsync(user, UserClaimTypes.ShopNum);
            if (claims == null)
                return 0;
            return int.Parse(claims.Value);
        }

        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new ApplicationException("There is no current user!");
            }

            return user;
        }
    }
}