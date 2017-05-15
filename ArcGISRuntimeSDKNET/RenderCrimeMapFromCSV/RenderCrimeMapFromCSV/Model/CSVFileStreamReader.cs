using System;
using System.Collections.Generic;
using System.IO;

namespace RenderCrimeMapFromCSV.Model
{
    internal class CSVFileStreamReader
    {
        public CSVFileStreamReader(string path)
        {
            CSVRowList = CreateRowListFromCSV(OpenCSVFile(path));
        }

        public IList<string> CSVRowList { get; } = new List<string>();

        private IList<string> CreateRowListFromCSV(FileStream fileStream)

        {
            using (fileStream)
            using (var reader = new StreamReader(fileStream))
            {
                var rowlist = new List<String>();
                while (!reader.EndOfStream)
                {
                    rowlist.Add(reader.ReadLine());
                }
                return rowlist;
            }
        }

        private FileStream OpenCSVFile(string path) => (!File.Exists((string)path)) ? null : File.OpenRead((string)path);
    }
}