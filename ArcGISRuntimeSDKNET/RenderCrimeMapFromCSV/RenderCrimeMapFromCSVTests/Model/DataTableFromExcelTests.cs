using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace RenderCrimeMapFromCSV.Model.Tests
{
    [TestClass()]
    public class DataTableFromExcelTests
    {
        [TestMethod()]
        public void GivenpathinputReturnDatatable()
        {
            //throw new NotImplementedException();
        }

        [TestMethod()]
        public void GivenpathinputReturnFirstColumnMonth()
        {
            Assert.AreEqual(true, GetExcelTable(2017).Columns.Contains("Area Affected"), "2017");
            Assert.AreEqual(true, GetExcelTable(2016).Columns.Contains("Area Affected"), "2016");
            Assert.AreEqual(true, GetExcelTable(2015).Columns.Contains("Area Affected"), "2015");
            Assert.AreEqual(true, GetExcelTable(2014).Columns.Contains("Area Affected"), "2014");
            Assert.AreEqual(true, GetExcelTable(2013).Columns.Contains("Area Affected"), "2013");
            Assert.AreEqual(true, GetExcelTable(2012).Columns.Contains("Area Affected"), "2012");
        }

        [TestMethod()]
        public void GivenPathInputReturnRowCount36()
        {
            Assert.AreEqual(36, GetExcelTable(2017).Rows.Count);
        }

        [TestMethod()]
        public void GivenDataTableExcelReturnStatesCSV()
        {
            var states = new StatesReader()
                            .ConstructStatesString(GetExcelTable(2017));
            // 56 if duplicates in the string
            // 43 if unique states
            Assert.AreEqual(27, states.Split(',').Length);
        }

        // TODO givenemptypath return empty datatable
        private static DataTable GetExcelTable(int year)
        {
            var path16 = $"data\\{year.ToString()}_Annual_Summary.xls";
            var datatable = new DataTableFromExcel(path16);
            return datatable.DataTableExcel;
        }
    }
}