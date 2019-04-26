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
        public static void CreateBasicReport(string reportPath, IEnumerable<TransactionModel> transactions, int reportDays, bool ShowCategorySummaries, bool ShowBalanceGraph, bool ShowMostExpensivePurchases, bool ReportInflowGraph, bool ReportOutflowGraph)
        {
            var fp = new FileInfo(reportPath);

            using (var f = new ExcelPackage(fp))
            {
                var ws = f.Workbook.Worksheets.Add("Budget Report");
                ws.Cells["A1"].Value = "Test A1";
                ws.Cells[2, 1].Value = "Test A2 Bold";
                ws.Cells[2, 1].Style.Font.Bold = true;
                f.Save();

                // column showing gain/loss per category over X days

                // line graph sources of inflow over X days

                // line graph sources of outflow over X days

                // show top 10 largest expenses in last X days

                // show top 10 largest gains in last X days
            }
        }        
    }
}
