using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
        public void GivenpathinputReturnFirstColumnMonth() =>
            new List<int>(Enumerable.Range(2012, 4)).ForEach(x =>
                Assert.AreEqual(true, GetExcelTable(x).Columns.Contains("Area Affected"), x.ToString()));

        [TestMethod()]
        public void GivenPathInputReturnRowCount36() => Assert.AreEqual(191, GetExcelTable(2017).Rows.Count);

        [TestMethod()]
        public void GivenDataTableExcelReturnStatesCSV() =>
            Assert.AreEqual(27, new StatesReader().ConstructStatesString(GetExcelTable(2017)).Split(',').Length);

        // TODO givenemptypath return empty datatable
        private static DataTable GetExcelTable(int year) =>
             new DataTableFromExcel($"data\\{year.ToString()}_Annual_Summary.xls").DataTableExcel;
    }
}