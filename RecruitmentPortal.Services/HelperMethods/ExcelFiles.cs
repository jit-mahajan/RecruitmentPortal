
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
                worksheet.Cells.LoadFromCollection(data, true);
                package.Save();
            }
            return fileInfo.FullName;
        }

    }
}
