using System;
using System.Collections.Generic;
using System.IO;

namespace RenderCrimeMapFromCSV.Model
{
    internal class CSVFileFromStreamReader
    {
        // TODO: Rename all classes to nouns
        public CSVFileFromStreamReader(string path)
        {
            var fileStream = OpenCSVFile(path);
            CreateRowListFromCSV(fileStream);
        }

        private IList<string> csvRowList;

        public IList<string> CSVRowList
        {
            //get { return csvRowList; }
            get { return csvRowList ?? new List<string>(); }
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

        private FileStream OpenCSVFile(string path)
        {
            var filepath = path;
            Console.Write(File.Exists(filepath));
            if (!File.Exists(filepath)) return null;

            return File.OpenRead(filepath);
        }
    }
}