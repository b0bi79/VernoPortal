using System.Linq;
using Abp.Localization;
using Abp.Net.Mail;
using Verno.Identity.Data;
using Verno.Identity.Settings;

namespace Verno.Identity.Migrations.SeedData
{
    public class DefaultSettingsCreator
    {
        private readonly IdentityDbContext _context;

        public DefaultSettingsCreator(IdentityDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            //Emailing
            AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "admin@ivoin.ru");
            AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "ivoin.ru mailer");

            //Languages
            AddSettingIfNotExists(LocalizationSettingNames.DefaultLanguage, "ru");
        }

        private void AddSettingIfNotExists(string name, string value)
        {
            if (_context.Settings.Any(s => s.Name == name && s.UserId == null))
            {
                return;
            }

            _context.Settings.Add(new Setting(null, name, value));
            _context.SaveChanges();
        }
    }
}