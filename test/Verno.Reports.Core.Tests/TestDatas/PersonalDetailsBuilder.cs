using System.Data;
using System.Linq;

namespace Verno.Reports.Tests.TestDatas
{
    public class PersonalDetailsBuilder
    {
        private DataTable _dataTable;

        public PersonalDetailsBuilder CreateNewTable()
        {
            _dataTable = new DataTable("CustomerPersonalDetails");
            _dataTable.Columns.AddRange(new[]
                {
                    new DataColumn("CustomerId", typeof(int)),
                    new DataColumn("CustomerName", typeof(string))
                });

            return this;
        }

        public PersonalDetailsBuilder AddStandardData(int numberOfRows = 3)
        {
            foreach (int i in Enumerable.Range(1, numberOfRows + 1))
            {
                AddRow(i, "Customer " + i);
            }

            return this;
        }

        public PersonalDetailsBuilder AddRow(int customerId, string customerName)
        {
            _dataTable.Rows.Add(customerId, customerName);

            return this;
        }

        public IDataReader ToDataReader()
        {
            return _dataTable.CreateDataReader();
        }
    }
}