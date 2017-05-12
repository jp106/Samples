using RenderCrimeMapFromCSV.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RenderCrimeMapFromCSV.Model.Tests
{
    [TestClass()]
    public class ReadCSVFileTests
    {
        [TestMethod()]
        public void ReadCSVFileTestDefaultConstructor()
        {
            var read = new CSVFileFromTextFieldParser();

            Console.WriteLine("start test");
            //var rowlist = read.CSVRowList;

            Assert.IsNotNull(read.RowList);
        }

        private TestContext testContext;

        public TestContext TestContext
        {
            get { return testContext; }
            set { testContext = value   ; }
        }


        [TestMethod]
        public void ReadCSVFileTestEmptyFilepathInput()
        {
            Console.WriteLine("empty file path input test");
            string rowlist;
            var read = new CSVFileFromTextFieldParser("");
            try
            {
                rowlist = read.RowList[0][0];
                var newl = rowlist.Split(',');
            }
            catch (Exception e)
            {
                Console.WriteLine("3 empty file path input test");
                Console.WriteLine(" error :" + e.Message);
                return;

            }
            Console.WriteLine("4 epty file path input test");
            Assert.Fail("dfgdfg");
        }

        [TestMethod()]
        public void ReadCSVFileTestContructorWithWrongFilepath()
        {
            var filepath = @"Data\cridata.csv";
            var read = new CSVFileFromTextFieldParser(filepath);
            Assert.AreEqual(1279,read.RowList.Count);
        }

        [TestMethod()]
        public void ReadCSVFileTestConstructorWithFilepath()
        {
            var filepath = @"Data\crimedata.csv";
            var read = new CSVFileFromTextFieldParser(filepath);
            Assert.IsTrue(read.RowList.Count>1);
        }

       
    }
}