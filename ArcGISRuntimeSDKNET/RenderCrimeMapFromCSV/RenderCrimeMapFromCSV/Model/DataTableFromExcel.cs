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
            //DataTableExcel = ConnecttoExcek(excelpath);
        }

        public DataTable DataTableExcel { get; }

        private void ConnectToDataSetDataTable(string path) => ConnecttoExcek(path);

        private void ConnecttoExcek(string path)
        {
            using (OleDbConnection connection = new OleDbConnection(ConstructConnectionString(path)))
            {
                OleDbCommand command = new OleDbCommand();
                // Set the Connection to the new OleDbConnection.
                command.Connection = connection;
                command.CommandText = "Select * from [Sheet1]";
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private string ConstructConnectionString(string filepath)
        {
            string ConnectionString = string.Empty;

            string fileExtension = Path.GetExtension(filepath);
            if (fileExtension == ".xls")
                ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="
                                    + filepath + ";"
                                    + "Extended Properties='Excel 8.0;HDR=YES;'";
            if (fileExtension == ".xlsx")
                ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source="
                                    + filepath + ";"
                                    + "Extended Properties='Excel 12.0 Xml;HDR=YES;'";
            return ConnectionString;
        }

        private DataTable GetDataTableFromOLECommand(OleDbCommand cmd)
        {
            var datatable = new DataTable("ExcelData");

            return datatable;
        }
    }
}