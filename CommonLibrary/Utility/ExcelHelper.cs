using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace CommonLibrary.Utility
{
    public class ExcelHelper
    {
        private static string _ConnString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;{1}'";
        private static string GetConnectionString(string filePath, bool isWithCaption, bool isAllToText, string extraProporty)
        {
            string prop = "";
            if (isWithCaption)
                prop = "HDR=Yes;";
            if (isAllToText)
                prop += "IMEX=1;";
            extraProporty = string.Concat(prop, extraProporty);
            return string.Format(_ConnString, filePath, extraProporty);
        }

        public static DataTable GetDataTable(string filePath, int sheetIndex, bool isWithCaption, string regionStartCell, string regionEndCell, string extraExcelPropery)
        {
            DataTable ret = new DataTable();
            OleDbConnection conn = new OleDbConnection(GetConnectionString(filePath, isWithCaption, true, extraExcelPropery));
            System.Data.OleDb.OleDbDataAdapter adapter;
            OleDbCommand cmd;
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(string.Concat("File not exist.Path:", filePath));
            }
            else
            {
                try
                {
                    conn.Open();
                    DataTable exShema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    string fstSheetName;
                    string cmdText;
                    fstSheetName = exShema.Rows[sheetIndex][2].ToString().Trim();
                    cmdText = string.Concat("SELECT * from [", fstSheetName);
                    if (!string.IsNullOrEmpty(regionStartCell) && !string.IsNullOrEmpty(regionEndCell))
                        cmdText = string.Concat(cmdText, regionStartCell, ":", regionEndCell);
                    cmdText = string.Concat(cmdText, "]");
                    cmd = new OleDbCommand(cmdText, conn);
                    adapter = new OleDbDataAdapter(cmd);
                    adapter.Fill(ret);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
            return ret;
        }
        public static DataTable GetDataTableFromFirstSheet(string filePath, bool isWithCaption)
        {
            return GetDataTable(filePath, 0, isWithCaption, "", "", "");
        }
    }
}
