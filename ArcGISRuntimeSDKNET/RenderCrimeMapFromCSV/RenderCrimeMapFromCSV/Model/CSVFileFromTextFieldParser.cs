using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;

namespace RenderCrimeMapFromCSV.Model
{
    public class CSVFileFromTextFieldParser
    {
        private IList<string[]> rowList = new List<string[]>();

        public CSVFileFromTextFieldParser(string filepath)
        {
            UseTextFieldParser(filepath);
        }

        public IList<string[]> RowList
        {
            get { return rowList; }
            set { rowList = value; }
        }

        private void UseOLEDB()
        {
        }

        private void UseTextFieldParser(string filepath)
        {
            if (!FileSystem.FileExists(filepath)) return;

            using (var parser = new TextFieldParser(filepath))
            {
                parser.Delimiters = new string[] { "," };
                var rows = new List<string[]>();
                while (true)
                {
                    string[] parts = parser.ReadFields();

                    if (parts == null)
                    {
                        break;
                    }
                    Console.WriteLine("{0} fields", parts.Length);
                    rows.Add(parts);
                }

                rowList = rows;
            }
        }
    }
}