using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RenderCrimeMapFromCSV.Model.Tests
{
    [TestClass()]
    public class ReadCSVFileTests
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
            var filepath = @"Data\crimedata.csv";
            var read = new CSVFileFromTextFieldParser(filepath);
            Assert.AreEqual(1279, read.RowList.Count);
        }        

        [TestMethod()]
        public void ReadCSVFileTestContructorWithWrongFilepath()
        {
            var wrongpath = @"Data\cridata.csv";
            var read = new CSVFileFromTextFieldParser(wrongpath);
            Assert.AreEqual(0, read.RowList.Count);
        }

        [TestMethod()]
        public void ReadCSVFileTestDefaultConstructorWithNullInput()
        {
            var read = new CSVFileFromTextFieldParser(null);
            Assert.IsNotNull(read.RowList);
        }
        [TestMethod]
        public void ReadCSVFileTestEmptyFilepathInput()
        {
            string rowlist;
            var read = new CSVFileFromTextFieldParser("");
            try
            {
                rowlist = read.RowList[0][0];
                var newl = rowlist.Split(',');
            }
            catch (Exception e)
            {
                return;
            }
            Assert.Fail("Class mishandled empty path.");
        }
    }
}