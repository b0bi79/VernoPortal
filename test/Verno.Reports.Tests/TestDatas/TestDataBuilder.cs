using System;
using System.Collections;
using System.Collections.Generic;
using Verno.Reports.EntityFrameworkCore;
using Verno.Reports.Models;

namespace Verno.Reports.Tests.TestDatas
{
    public class TestDataBuilder
    {
        private readonly ReportsDbContext _context;

        public TestDataBuilder(ReportsDbContext context)
        {
            _context = context;
        }

        public void Build()
        {
            CreateConnections();
            CreateFormats();
            CreateReports();
        }

        private void CreateConnections()
        {
            CreateConnection("INFOR_SPB", "SqlServer", "Server=10.53.2.21;Database=SCPRD;Uid=wmwhse1;Pwd=WMwhSql1;");
            CreateConnection("ION10MAIN_Eff", "SqlServer", "Server=.;Database=shEffSQL;user=sa;password=44");
            CreateConnection("ION10MAIN_Mig", "SqlServer", "Server=.;Database=shMigSQL;user=sa;password=44");
        }

        private void CreateConnection(string id, string providerName, string connStr)
        {
            _context.ReportConnections.Add(new ReportConnection(id, connStr, providerName));
        }

        private void CreateFormats()
        {
            CreateFormat("HTML_TPL", "� Html", "XslTransform", null, "HTML", null);
            CreateFormat("XLS_AUTO", "� Excel", "ImportFromSql", null, "XLS", null);
            CreateFormat("JSON", "���������", "JsonTransform", null, null, null);
            CreateFormat("TEST", "���������", "TestTransform", null, "XLS", null);
        }

        private void CreateFormat(string id, string displayText, string generateUtil, string mimeType, string arguments, string description)
        {
            _context.OutFormats.Add(new OutFormat(id, displayText, generateUtil, mimeType, arguments, description));
        }

        private void CreateReports()
        {
            CreateReport("��������", "������������ �� �������", "INFOR_SPB", "OrderComplectation.sql", null,
                new[]
                {
                    new ReportColumn("picker", "�������������", aggregate: "GROUP"),
                    new ReportColumn("date", "����", "{0:dd.MM.yyyy}", aggregate: "GROUP"),
                    new ReportColumn("ORDERKEY", "�����"),
                    new ReportColumn("UOMQTY", "��������", aggregate: "SUM"),
                    new ReportColumn("ENDTIME", "���������", "HH:mm:ss"),
                    new ReportColumn("dur", "�������-��", "HH:mm:ss"),
                    new ReportColumn("test", "-", "HH:mm:ss", "#/reports/Otchet_za_vchera?username=admin&d={{date}}",
                        "UOMQTY/CASECNT", "SUM(UOMQTY)/SUM(CASECNT)", false, 1),
                    new ReportColumn("testdup", "�������� �������", formula: "test"),
                },
                new[]
                {
                    new ReportOutFormat("XLS_AUTO", displayText: "�������� � ������"),
                },
                new[]
                {
                    new RepParameter("period", "�� ������", "periodtime", DisplayType.PeriodDateTime, value: "wb|we 23:59")
                });
            CreateReport("��������", "����� �� �����", "ION10MAIN_Mig", null, "Report_DayTurnover",
                new[]
                {
                    new ReportColumn("Region", "������", aggregate: "GROUP"),
                    new ReportColumn("Locality", "������", aggregate: "GROUP"),
                    new ReportColumn("ShopNum", "�������"),
                    new ReportColumn("CurrPlan", "����"),
                    new ReportColumn("CurrSumm", "����"),
                },
                new[]
                {
                    new ReportOutFormat("XLS_AUTO", displayText: "�������� � ������"),
                },
                new[]
                {
                    new RepParameter("username", "������������", TypeCode.String, DisplayType.TextBox),
                    new RepParameter("d", "", "period", DisplayType.PeriodDate, value: "wb|we"),
                });
            var report = CreateReport("��������", "������� ����������", "ION10MAIN_Mig", "ParametersTest.sql", null,
                new[]
                {
                    new ReportColumn("username", "username"),
                    new ReportColumn("filial", "filial"),
                    new ReportColumn("project", "project"),
                    new ReportColumn("role", "role"),
                    new ReportColumn("ListBoxValue", "ListBoxValue", tableNo: 1),
                },
                new[]
                {
                    new ReportOutFormat("HTML_TPL", "ParamsDemonstration_html.xsl"),
                    new ReportOutFormat("XLS_AUTO", displayText: "�������� � ������"),
                    new ReportOutFormat("TEST", displayText: "Test"),
                },
                new[]
                {
                    new RepParameter("str", "������", TypeCode.String, DisplayType.TextBox, value: "�������������� ������"),
                    new RepParameter("integer", "�����", TypeCode.Int32, DisplayType.TextBox, true, "#,##0", "3215325"),
                    new RepParameter("numeric", "�������", TypeCode.Double, DisplayType.TextBox, false, "#,##0.0#", "10.46548"),
                    new RepParameter("date", "����", TypeCode.DateTime, DisplayType.Date, true, "d MMM yyyy", "t-10d"),
                    new RepParameter("dtime", "���������", TypeCode.DateTime, DisplayType.DateTime, true, "dd.MM HH:mm", "n-2h"),
                    new RepParameter("period", "������", "period", DisplayType.PeriodDate, value: "mb|me"),
                    new RepParameter("truefalse", "����������", TypeCode.Boolean, DisplayType.CheckBox, value: "��"),
                    new RepParameter("ComboBox", "� ���������� �������", TypeCode.Int32, DisplayType.ComboBox, true, null, "3", "LAZY{SELECT ID, Name FROM Regions}"),
                    new RepParameter("ListBox", "������������� �����", TypeCode.Int32, DisplayType.ListBox, true, null, "2,4", "[1:'����', 2:'���', 3:'���', 4:'������']"),
                    new RepParameter("RadioButton", "�����", TypeCode.String, DisplayType.RadioButton, true, null, "������", "['������', '������', '������']"),
                });

            _context.SaveChanges();
            _context.UserPermissions.Add(new UserPermission(report.Id, TestIdentityBuilder.User1.Id));
            _context.SaveChanges();
        }

        private Report CreateReport(string category, string name, string connectionId, string sqlFile, string sqlProc,
            IEnumerable<ReportColumn> columns, IEnumerable<ReportOutFormat> formats, IEnumerable<RepParameter> parameters)
        {
            var result = new Report(category, name, connectionId, sqlFile, sqlProc);
            var report = _context.Reports.Add(result).Entity;
            foreach (var column in columns)
            {
                report.Columns.Add(column);
            }
            foreach (var format in formats)
            {
                report.ReportOutFormats.Add(format);
            }
            foreach (var parameter in parameters)
            {
                report.Parameters.Add(parameter);
            }
            return result;
        }
    }
}