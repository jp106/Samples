using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RenderCrimeMapFromCSV.Model.Tests
{
    [TestClass()]
    public class CSVFileFromTextFieldParserTests
    {
        private TestContext testContext;

        public TestContext TestContext
        {
            get { return testContext; }
            set { testContext = value; }
        }

        [TestMethod]
        public void CSVFileFromStreamReaderTestConstructorWithRightFilepath()
        {
            var validpath = @"Data\crimedata.csv";
            var read = new CSVFileTextFieldParser(validpath);
            Assert.AreEqual(1279, read.RowList.Count);
        }        

        [TestMethod()]
        public void ReadCSVFileTestContructorWithWrongFilepath()
        {
            var invalidpath = @"Data\cridata.csv";
            var read = new CSVFileTextFieldParser(invalidpath);
            Assert.AreEqual(0, read.RowList.Count);
        }

        [TestMethod()]
        public void ReadCSVFileTestDefaultConstructorWithNullInput()
        {
            var read = new CSVFileTextFieldParser(null);
            Assert.IsNotNull(read.RowList);
        }
        [TestMethod]
        public void ReadCSVFileTestEmptyFilepathInput()
        {
            string rowlist;
            var read = new CSVFileTextFieldParser("");
            try
            {
                rowlist = read.RowList[0][0];
                var newl = rowlist.Split(',');
            }
            catch (Exception)
            {
                return;
            }
            Assert.Fail("Class mishandled empty path.");
        }
    }
}