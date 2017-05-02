using System;
using System.Collections.Generic;
using System.IO;

namespace RenderCrimeMapFromCSV.Model
{
    internal class ReadCSVFile
    {
        public ReadCSVFile()
        {
        }

        public ReadCSVFile(string filepath)
        {
            var fileStream = OpenCSVFile(filepath);
            CreateRowListFromCSV(fileStream);
        }

        private IList<string> csvRowList;

        public IList<string> CSVRowList
        {
            get { return csvRowList; }
        }

        private FileStream OpenCSVFile(string path)
        {
            var filepath = @"Data\crimedata.csv";
            Console.Write(File.Exists(filepath));
            if (!File.Exists(filepath)) return null;

            return File.OpenRead(filepath);
        }

        private void CreateRowListFromCSV(FileStream fileStream)

        {
            using (fileStream)
            using (var reader = new StreamReader(fileStream))
            {
                var rowlist = new List<String>();
                while (!reader.EndOfStream)
                {
                    rowlist.Add(reader.ReadLine());
                }

                csvRowList = rowlist;
            }
        }
    }
}