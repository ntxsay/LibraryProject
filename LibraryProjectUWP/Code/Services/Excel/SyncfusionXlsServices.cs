using LibraryProjectUWP.Code.Helpers;
using LibraryProjectUWP.Code.Services.Logging;
using LibraryProjectUWP.ViewModels.Book;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace LibraryProjectUWP.Code.Services.Excel
{
    public class SyncfusionXlsServices 
    {
        readonly StorageFile _file;
        public SyncfusionXlsServices()
        {

        }

        public SyncfusionXlsServices(StorageFile file)
        {
            _file = file;
        }

        public async Task<IEnumerable<string>> GetExcelSheetsName(StorageFile file = null)
        {
            try
            {
                StorageFile excelFile = file ?? _file;
                //Create an instance of ExcelEngine
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    //Opens the workbook. 
                    IWorkbook workbook = await application.Workbooks.OpenAsync(excelFile);

                    List<string> sheetNames = new List<string>();
                    foreach (var worksheet in workbook.Worksheets)
                    {
                        var worksheetName = worksheet.Name;
                        if (!worksheetName.IsStringNullOrEmptyOrWhiteSpace())
                        {
                            sheetNames.Add(worksheet.Name);
                        }
                    }
                    return sheetNames;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return Enumerable.Empty<string>();
            }
        }

        public async Task<DataTable> ImportExcelToDatatable(string sheetName, StorageFile file = null)
        {
            try
            {
                if (sheetName.IsStringNullOrEmptyOrWhiteSpace())
                {
                    return null;
                }

                StorageFile excelFile = file ?? _file;
                
                //Create an instance of ExcelEngine
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    //Opens the workbook. 
                    IWorkbook workbook = await application.Workbooks.OpenAsync(excelFile);

                    //Access first worksheet from the workbook.
                    IWorksheet worksheet = workbook.Worksheets.FirstOrDefault(f => f.Name == sheetName);

                    //Create a new DataTable.
                    DataTable dt = new DataTable();

                    //Loop through the Worksheet rows.
                    bool firstRow = true;
                    int countRow = 1;
                    foreach (IRange row in worksheet.Rows)
                    {

                        //Use the first row to add columns to DataTable.
                        if (firstRow)
                        {
                            dt.Columns.Add("#");
                            foreach (IRange cell in row.Cells)
                            {
                                var chars = cell.AddressLocal.ToCharArray().ToList();
                                for (int y = 0; y < chars.Count; y++)
                                {
                                    var charc = chars[y];
                                    if (!char.IsLetter(charc))
                                    {
                                        chars.Remove(charc);
                                        y = 0;
                                        continue;
                                    }
                                }

                                string columnLetter = new string(chars.ToArray());
                                dt.Columns.Add(columnLetter);
                            }

                            firstRow = false;
                        }

                        //Add rows to DataTable.
                        dt.Rows.Add();

                        var cellf = row.Cells;
                        dt.Rows[dt.Rows.Count - 1][0] = $"{countRow}";
                        countRow++;

                        int i = 1;
                        foreach (var cell in cellf)
                        {
                            dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                            i++;
                        }

                        firstRow = false;
                    }

                    return dt;
                }
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Logs.Log(ex, m);
                return null;
            }
        }
    }
}
