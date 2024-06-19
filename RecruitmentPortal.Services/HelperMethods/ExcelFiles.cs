
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace RecruitmentPortal.Services.HelperMethods
{
    public class ExcelFiles
    {
        public static string ExportToExcel<T>(List<T> data, string sheetName)
        {
            var fileInfo = new FileInfo($"{sheetName}.xlsx");

            using (var package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);

                // Write headers
                var properties = typeof(T).GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = properties[i].Name;
                }

                // Write data
                for (int row = 0; row < data.Count; row++)
                {
                    var item = data[row];
                    for (int col = 0; col < properties.Length; col++)
                    {
                        var value = properties[col].GetValue(item);

                        if (value is DateTime dateTimeValue)
                        {
                            worksheet.Cells[row + 2, col + 1].Value = dateTimeValue;
                            worksheet.Cells[row + 2, col + 1].Style.Numberformat.Format = "yyyy-MM-dd HH:mm:ss";
                        }
                        else
                        {
                            worksheet.Cells[row + 2, col + 1].Value = value;
                        }
                    }
                }

                package.Save();
            }

            return fileInfo.FullName;

        }
    }
}
