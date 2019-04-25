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
            if (File.Exists(reportPath))
            {
                File.Delete(reportPath);
            }

            var fp = new FileInfo(reportPath);

            // get all distinct categories
            List<string> categories = transactions.Select(t => t.Category).Distinct().ToList();

            // get gain/loss for each category

            using (var f = new ExcelPackage(fp))
            {
                var ws = f.Workbook.Worksheets.Add($"My {reportDays}-day Budget Report");


                // category summmaries - column showing gain/loss per category over X days
                ws.Cells["A1:B1"].Merge = true;
                ws.Cells["A1:B1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells["A1:B1"].Value = "Category Summaries";
                ws.Cells["A1:B1"].Style.Font.Bold = true;

                ws.Cells["A2"].Value = "Category";
                ws.Cells["B2"].Value = "Gain/Loss";
                ws.Cells["A2:B2"].Style.Font.Bold = true;
                
                ws.Cells["A3"].Value = "testCategory";
                ws.Cells["B3"].Value = "$-1000";
                

                // balance graph - show balance per day over last X days

                // show top 10 largest expenses in last X days

                // line graph sources of inflow over X days

                // line graph sources of outflow over X days




                ws.Column(1).Width = 20;
                ws.Column(2).Width = 20;
                f.Save();                
            }
        }        
    }
}
