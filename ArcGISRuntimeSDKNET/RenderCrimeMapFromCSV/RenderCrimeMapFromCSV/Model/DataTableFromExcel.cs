using System;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace RenderCrimeMapFromCSV.Model
{
    public class DataTableFromExcel
    {
        public DataTableFromExcel(string excelpath)
        {
            DataTableExcel = ConnecttoExcel(excelpath);
        }

        public DataTable DataTableExcel { get; } = new DataTable("OutageTable");

        private void ConnectToDataSetDataTable(string path) => ConnecttoExcel(path);

        private DataTable ConnecttoExcel(string path)
        {
            using (OleDbConnection connection = new OleDbConnection(ConstructConnectionString(path)))
            {
                // Set the Connection to the new OleDbConnection.
                const string querystring = "Select * from [Sheet1$] where Month <>''";
                try
                {
                    connection.Open();
                    var dataAdapter = new OleDbDataAdapter(querystring, connection);
                    var ds = new DataSet();
                    dataAdapter.Fill(ds, "OutageTable");
                    return ds?.Tables[0];
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return DataTableExcel;
                }
            }
        }

        private string ConstructConnectionString(string filepath)
            => (Path.GetExtension(filepath) == ".xlsx")? 
                connectionStringXLS(filepath, string.Empty, Path.GetExtension(filepath)) :
                connectionSTringXLSX(filepath, string.Empty, Path.GetExtension(filepath));
        
        private static string connectionSTringXLSX(string filepath, string connectionString, string fileExtension)
            => "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="
                                    + filepath + ";"
                                    + "Extended Properties='Excel 12.0 Xml;HDR=YES;'";
        
        private static string connectionStringXLS(string filepath, string connectionString, string fileExtension)
           =>  "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="
                                    + filepath + ";"
                                    + "Extended Properties='Excel 8.0;HDR=YES;'";
        
    }
}