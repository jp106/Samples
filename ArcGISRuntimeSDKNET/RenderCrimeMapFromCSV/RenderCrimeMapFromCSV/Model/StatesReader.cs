using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RenderCrimeMapFromCSV.Model
{
    public class StatesReader
    {
        public HashSet<string> states { get; internal set; }

        public string ConstructStatesString(DataTable dt)
        {
            var statesh = new HashSet<string>();
            dt.AsEnumerable().ToList()
                .ForEach(r =>
                getStateName(r, ref statesh));
            return string.Join(",", statesh.Select(x => x = $"'{x}'"));
        }

        private void getStateName(DataRow r, ref HashSet<string> states)
        {
            var statename = r.ItemArray[r.Table.Columns.IndexOf("Area affected")]
                             .ToString();
            statename = filterCountyNames(statename);
            states = states.Contains(statename) ? states :
                new HashSet<string>(states.Union(splitStateNames(statename)));
        }

        private static string filterCountyNames(string statename)
        {
            return statename.Contains(';') ?
                    statename.Split(':')?[0]
                    : statename.Trim(':').Replace(':', ',');
        }

        private HashSet<string> splitStateNames(string statename)
        {
            var rowstates = new HashSet<string>();
            statename.Split(',')
                            .Where(x => !string.IsNullOrEmpty(x)).ToList()
                            .ForEach(x => rowstates.Add(x.Trim()));
            return rowstates;
        }

        private static void getgeometry(DataRow r, List<KeyValuePair<string, object>> attributes, DataColumnCollection columns)
        {
            r.ItemArray.Select((x, i) => new { X = x, i = i })
                            .ToList()
                            .ForEach(x => attributes
                            .Add(new KeyValuePair<string, object>(columns[x.i].ColumnName, x.X)));
        }
    }
}