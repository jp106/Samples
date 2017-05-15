using System;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace RenderCrimeMapFromCSV.Model
{
    internal class DataTableFromExcel
    {
        public DataTableFromExcel(string excelpath)
        {
            // TODO connect and fill datatable
            DataTableExcel = ConnecttoExcek(excelpath);
        }

        public DataTable DataTableExcel { get; }

        private void ConnectToDataSetDataTable(string path) => ConnecttoExcek(path);

        private DataTable ConnecttoExcek(string path)
        {
            using (OleDbConnection connection = new OleDbConnection(ConstructConnectionString(path)))
            {
                // Set the Connection to the new OleDbConnection.
                const string querystring = "Select * from [Sheet1$]";
                try
                {
                    connection.Open();
                   var  dataAdapter = new OleDbDataAdapter(querystring,connection);
                    var ds = new DataSet();
                    dataAdapter.Fill(ds, "outagetable");
                    return ds?.Tables[0];
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        private string ConstructConnectionString(string filepath)
        {
            string connectionString = string.Empty;

            string fileExtension = Path.GetExtension(filepath);
            if (fileExtension == ".xls")
                connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="
                                    + filepath + ";"
                                    + "Extended Properties='Excel 8.0;HDR=YES;'";
            if (fileExtension == ".xlsx")
                connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="
                                    + filepath + ";"
                                    + "Extended Properties='Excel 12.0 Xml;HDR=YES;'";
            return connectionString;
        }

       
    }
}