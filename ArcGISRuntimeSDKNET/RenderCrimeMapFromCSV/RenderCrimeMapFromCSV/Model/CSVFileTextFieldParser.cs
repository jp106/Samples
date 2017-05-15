using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.Diagnostics;

namespace RenderCrimeMapFromCSV.Model
{
    public class CSVFileTextFieldParser
    {
        private IList<string[]> rowList = new List<string[]>();

        public CSVFileTextFieldParser(string filepath)
        {
            RowList = UseTextFieldParser(filepath);
        }

        public IList<string[]> RowList { get; } = new List<string[]>();

        private void UseOLEDB()
        {
        }

        private IList<string[]> UseTextFieldParser(string filepath)
        {
            var rows = new List<string[]>();
            if (!FileSystem.FileExists(filepath)) return rows;

            using (var parser = new TextFieldParser(filepath))
            {
                parser.Delimiters = new string[] { "," };
                while (true)
                {
                    string[] parts = parser.ReadFields();

                    if (parts?.Length > 0)
                    {
                        Debug.Print($"{parts.Length} fields");
                        rows.Add(parts);
                        continue;
                    }
                    break;
                }

                return rows;
            }
        }
    }
}