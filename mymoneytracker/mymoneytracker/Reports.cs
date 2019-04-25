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

            using (var f = new ExcelPackage(fp))
            {
                var ws = f.Workbook.Worksheets.Add($"My {reportDays}-day Budget Report");


                /////
                // category summmaries - column showing gain/loss per category over X days
                ws.Cells[1, 1, 1, 2].Merge = true;
                ws.Cells[1, 1, 1, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                ws.Cells[1, 1, 1, 2].Value = "Category Summaries";
                ws.Cells[1, 1, 1, 2].Style.Font.Bold = true;
                ws.Cells[2,1].Value = "Category";
                ws.Cells[2,2].Value = "Gain/Loss";
                ws.Cells[2,1,2,2].Style.Font.Bold = true;

                // get all distinct categories
                DateTime cutoffDate = DateTime.Now.AddDays(-reportDays);
                List<string> categories = transactions.Where(t => (t.Date.CompareTo(cutoffDate) > 0)).Select(t => t.Category).Distinct().ToList();
                
                int i = 3;
                foreach (var category in categories)
                {
                    // sum the changes over last X days                    
                    decimal changed = (from t in transactions where ((t.Category == category) && (t.Date.CompareTo(cutoffDate) >= 0)) select t.Amount).Sum();

                    var c = category;
                    if (category == "")
                    {
                        c = "Uncategorized";
                    }

                    ws.Cells[i, 1].Value = c;
                    ws.Cells[i, 2].Value = changed;
                    i++;
                }

                /////
                // balance graph - show balance per day over last X days

                /////
                // show top 10 largest expenses in last X days

                /////
                // line graph sources of inflow over X days

                /////
                // line graph sources of outflow over X days




                ws.Column(1).Width = 20;
                ws.Column(2).Width = 20;
                f.Save();                
            }
        }      
    }
}
