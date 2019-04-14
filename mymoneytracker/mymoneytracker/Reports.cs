using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OfficeOpenXml;

namespace mymoneytracker
{
    class Reports
    {
        public static void CreateBasicReport(string reportPath)
        {
            var fp = new FileInfo(reportPath);

            using (var f = new ExcelPackage(fp))
            {
                var ws = f.Workbook.Worksheets.Add("Budget Report");
                ws.Cells["A1"].Value = "Test A1";
                ws.Cells[2, 1].Value = "Test A2 Bold";
                ws.Cells[2, 1].Style.Font.Bold = true;
                f.Save();
            }
        }        
    }
}
