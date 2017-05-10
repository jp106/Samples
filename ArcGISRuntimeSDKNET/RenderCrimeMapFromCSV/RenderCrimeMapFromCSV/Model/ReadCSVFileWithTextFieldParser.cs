using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;

namespace RenderCrimeMapFromCSV.Model
{
    public class ReadCSVFileWithTextFieldParser
    {
        private IList<string[]> rowList;

        public ReadCSVFileWithTextFieldParser()
        {
        }

        public ReadCSVFileWithTextFieldParser(string filepath)
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