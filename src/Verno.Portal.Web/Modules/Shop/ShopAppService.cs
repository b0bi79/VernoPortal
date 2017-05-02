using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.AutoMapper;
using Abp.Domain.Uow;
using Abp.UI;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Spire.Doc;
using Verno.ActionResults;
using Verno.Configuration;
using Verno.Portal.Web.DataAccess;
using Verno.Portal.Web.Models;
using Verno.ShInfo.EntityFrameworkCore.Repositories;
using Verno.ShInfo.Models;

namespace Verno.Portal.Web.Modules.Shop
{
    public interface IShopAppService
    {
        Task<ListResultDto<ProductionCalc>> GetProductionCalculator(int shopNum);
        Task<ActionResult> GetProductionCalculatorToExcel(int shopNum);
        Task<ActionResult> StickerFile(int shopNum, int vidTovara);
        Task<ActionResult> StickerFile(int shopNum, int[] ids, int[] qtys);
    }

    [Route("api/services/app/shop")]
    [AbpAuthorize(ShopPermissionNames.Documents_Shop)]
    public class ShopAppService : AppServiceBase, IShopAppService
    {
        private readonly ShopDbContext _context;
        private readonly ProizvMagSpecItemRepository _specItemRepository;
        private readonly ProizvMagSpecRepository _specRepository;
        private readonly ProektRepository _proektRepository;
        private readonly NomenklaturaRepository _nomenklatura;
        private readonly AppSettings _appSettings;
        private readonly KlientyRepository _klienty;
        private readonly OperationsLogRepository _logsRepository;

        public ShopAppService(ShopDbContext context, IOptions<AppSettings> appSettings,
            ProizvMagSpecItemRepository specItemRepository,
            ProizvMagSpecRepository specRepository,
            ProektRepository proektRepository,
            NomenklaturaRepository nomenklatura,
            KlientyRepository klienty, OperationsLogRepository logsRepository)
        {
            _context = context;
            _specItemRepository = specItemRepository;
            _specRepository = specRepository;
            _proektRepository = proektRepository;
            _nomenklatura = nomenklatura;
            _appSettings = appSettings.Value;
            _klienty = klienty;
            _logsRepository = logsRepository;
        }

        #region Implementation of IShopAppService

        [Route("production-spec")]
        [UnitOfWork(isTransactional: false)]
        [AbpAuthorize(ShopPermissionNames.Documents_Shop_Production_Manage)]
        public async Task<ListResultDto<ProizvMagSpecDto>> GetProductionSpec()
        {
            var user = await GetCurrentUserAsync();
            await _logsRepository.InsertAsync(new OperationsLog("GetProductionSpec", userId: user.Id));

            var result = _specRepository.GetAll();

            return new ListResultDto<ProizvMagSpecDto>((await result.Take(800).ToListAsync()).MapTo<List<ProizvMagSpecDto>>());
        }

        [Route("production-spec/{specId}")]
        [UnitOfWork(isTransactional: false)]
        [AbpAuthorize(ShopPermissionNames.Documents_Shop_Production_Manage)]
        public async Task<ProizvMagSpecDto> GetProductionSpec(int specId)
        {
            var user = await GetCurrentUserAsync();
            await _logsRepository.InsertAsync(new OperationsLog("GetProductionSpec/"+specId, userId: user.Id));

            var result = await _specRepository.GetAsync(specId);
            result.Items = await _specItemRepository.GetForSpec(specId).ToListAsync();

            return result.MapTo<ProizvMagSpecDto>();
        }

        [Route("production-spec/{specId}/items")]
        [UnitOfWork(isTransactional: false)]
        [AbpAuthorize(ShopPermissionNames.Documents_Shop_Production_Manage)]
        public async Task<ListResultDto<ProizvMagSpecItemDto>> GetProductionSpecItems(int specId)
        {
            var result = _specItemRepository.GetForSpec(specId);
            return new ListResultDto<ProizvMagSpecItemDto>((await result.ToListAsync()).MapTo<List<ProizvMagSpecItemDto>>());
        }

        [HttpPut]
        [Route("production-spec")]
        [UnitOfWork]
        [AbpAuthorize(ShopPermissionNames.Documents_Shop_Production_Manage)]
        public async Task<ProizvMagSpecDto> PutProductionSpec(ProizvMagSpecDto input)
        {
            var user = await GetCurrentUserAsync();
            await _logsRepository.InsertAsync(new OperationsLog("PutProductionSpec", input.ToString(), user.Id));

            var spec = input.MapTo<ProizvMagSpec>();

            input.Id = await _specRepository.InsertOrUpdateAndGetIdAsync(spec);
            foreach (var item in input.Items)
            {
                var specItem = item.MapTo<ProizvMagSpecItem>();
                specItem.ProizvMagSpecId = input.Id;
                specItem.VidTovara = item.Tovar.Id;
                item.Id = await _specItemRepository.InsertOrUpdateAndGetIdAsync(specItem);
            }

            return input;
        }

        [Route("production-spec/{specId}/{specItemId}")]
        [UnitOfWork]
        [AbpAuthorize(ShopPermissionNames.Documents_Shop_Production_Manage)]
        public async Task DeleteProductionSpecItem(int specId, int specItemId)
        {
            await _specItemRepository.DeleteAsync(specItemId);
        }

        [Route("production-spec/names")]
        [UnitOfWork]
        [AbpAuthorize(ShopPermissionNames.Documents_Shop_Production_Manage)]
        public async Task<ListResultDto<Nomenklatura>> GetProductionNames(int proekt)
        {
            var list = await _context.GetProductionTovars(proekt);
            return new ListResultDto<Nomenklatura>(list.MapTo<List<Nomenklatura>>());
        }

        [Route("{shopNum}/production-calculator")]
        [UnitOfWork(isTransactional: false)]
        [AbpAuthorize(ShopPermissionNames.Documents_Shop_Production)]
        public async Task<ListResultDto<ProductionCalc>> GetProductionCalculator(int shopNum)
        {
            var user = await GetCurrentUserAsync();
            var userShopNum = await GetUserShopNum(user);
            shopNum = userShopNum == 0 ? shopNum : userShopNum;
            if (shopNum == 0)
                throw new UserFriendlyException("Пользователю не присвоен номер магазина.");

            await _logsRepository.InsertAsync(new OperationsLog("GetProductionCalculator", userId: user.Id));

            return new ListResultDto<ProductionCalc>(await _context.GetProductionCalculator(shopNum));
        }

        [Route("{shopNum}/production-calculator/excel")]
        [UnitOfWork(isTransactional: false)]
        [AbpAuthorize(ShopPermissionNames.Documents_Shop_Production)]
        public async Task<ActionResult> GetProductionCalculatorToExcel(int shopNum)
        {
            var user = await GetCurrentUserAsync();
            var userShopNum = await GetUserShopNum(user);
            shopNum = userShopNum == 0 ? shopNum : userShopNum;
            if (shopNum == 0)
                throw new UserFriendlyException("Пользователю не присвоен номер магазина.");

            await _logsRepository.InsertAsync(new OperationsLog("GetProductionCalculatorToExcel", userId: user.Id));

            var datas = await _context.GetProductionCalculator(shopNum);
            using (var template = File.OpenRead(Path.Combine(_appSettings.XlReportTemplatesPath, "BakeCalculator.xlsx")))
            {
                var workbook = new XLWorkbook(template);
                var xlsxTemplate = new ClosedXML.Report.XLTemplate(workbook);
                xlsxTemplate.AddVariable("table", datas);
                xlsxTemplate.AddVariable("shopNum", shopNum);
                xlsxTemplate.Generate();

                return new MemoryStreamResult("BakeCalculator.xlsx", file => workbook.SaveAs(file));
            }
        }

        [HttpGet]
        [Route("{shopNum}/nomenklatura/{vidTovara}/sticker")]
        [UnitOfWork(isTransactional: false)]
        [AbpAuthorize(ShopPermissionNames.Documents_Shop_Production)]
        public async Task<ActionResult> StickerFile(int shopNum, int vidTovara)
        {
            var userShopNum = await GetUserShopNum();
            shopNum = userShopNum == 0 ? shopNum : userShopNum;
            if (shopNum == 0)
                throw new UserFriendlyException("Пользователю не присвоен номер магазина.");

            var shop = await _klienty.GetByShopNumAsync(shopNum);
            if (shop == null)
                throw new UserFriendlyException($"Не найден магазин {shopNum}.");

            var tovConf = await _context.KonfProizvMag.FirstOrDefaultAsync(x => x.VidTovara == vidTovara);
            //var tovar = await _nomenklatura.GetAsync(vidTovara);
            if (tovConf != null && tovConf.Etiketka != null/* && tovar != null*/)
            {
                var filepath = Path.Combine(ServPath, tovConf.Etiketka);
                if (File.Exists(filepath) && Path.GetExtension(filepath) == ".docx")
                {
                    Document document = new Document();
                    document.LoadFromFile(filepath);

                    document.Replace("{DateTime}", DateTime.Now.ToString("dd.MM.yyyy HH:mm"), false, true);
                    document.Replace("{ProdAddr}", shop.Adres, false, true);

                    document.Protect(ProtectionType.AllowOnlyReading, "EFDE775C-56F6-47ED-8C79-0AC2C027D6B1");
                    return new MemoryStreamResult(tovConf.Etiketka, file => document.SaveToStream(file, FileFormat.Docx));
                }
            }
            return new NotFoundResult();
        }

        [HttpGet]
        [Route("{shopNum}/nomenklatura/sticker")]
        [UnitOfWork(isTransactional: false)]
        [AbpAuthorize(ShopPermissionNames.Documents_Shop_Production)]
        public async Task<ActionResult> StickerFile(int shopNum, [FromQuery] int[] ids, [FromQuery] int[] qtys)
        {
            if (ids.Length != qtys.Length)
                throw new UserFriendlyException("Неверный формат вызова.");

            var userShopNum = await GetUserShopNum();
            shopNum = userShopNum == 0 ? shopNum : userShopNum;
            if (shopNum == 0)
                throw new UserFriendlyException("Пользователю не присвоен номер магазина.");

            var shop = await _klienty.GetByShopNumAsync(shopNum);
            if (shop == null)
                throw new UserFriendlyException($"Не найден магазин {shopNum}.");

            var tovConf = await _context.KonfProizvMag
                .Where(x => x.Filial == shop.Filial
                            && x.Etiketka != null && ids.Contains(x.VidTovara))
                .Select(x => new {x.VidTovara, x.Etiketka})
                .Distinct()
                .ToDictionaryAsync(x=>x.VidTovara, x=>x.Etiketka);
            Document document = new Document();

            for (var i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                var filepath = Path.Combine(ServPath, tovConf[id]);
                if (!File.Exists(filepath) || Path.GetExtension(filepath) != ".docx")
                {
                    var section = document.AddSection();
                    var para = section.AddParagraph();
                    para.AppendText($"Не найден файл этикетки {tovConf[id]}.");
                    continue;
                }

                using (Document stickTpl = new Document())
                {
                    stickTpl.LoadFromFile(filepath);

                    stickTpl.Replace("{DateTime}", DateTime.Now.ToString("dd.MM.yyyy HH:mm"), false, true);
                    stickTpl.Replace("{ProdAddr}", shop.Adres, false, true);

                    for (int j = 0; j < qtys[i]; j++)
                    {
                        document.ImportContent(stickTpl);
                    }
                    stickTpl.Close();
                }
            }

            document.Protect(ProtectionType.AllowOnlyReading, "EFDE775C-56F6-47ED-8C79-0AC2C027D6B1");
            return new MemoryStreamResult($"stickers_{DateTime.Now:d}.docx", file => document.SaveToStream(file, FileFormat.Docx));
        }

        #endregion

        private string _servPath;
        public string ServPath => _servPath ?? (_servPath = _appSettings.PrintTemplatesPath);

        public class IdQty
        {
            private int id;
            private int qty;
        }
    }
}
