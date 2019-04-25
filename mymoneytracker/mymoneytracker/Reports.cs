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
        public static void CreateBasicReport(string reportPath, IEnumerable<TransactionModel> transactions, int reportDays, Decimal currentBalance, bool ShowCategorySummaries, bool ShowBalanceGraph, bool ShowMostExpensivePurchases, bool ReportInflowGraph, bool ReportOutflowGraph)
        {

            var bgOffset = 0;
            var i = 0;
            if (ShowCategorySummaries)
            {
                bgOffset += 2;
            }

            if (File.Exists(reportPath))
            {
                File.Delete(reportPath);
            }

            var fp = new FileInfo(reportPath);

            using (var f = new ExcelPackage(fp))
            {
                var ws = f.Workbook.Worksheets.Add($"My {reportDays}-day Budget Report");

                // get all distinct categories
                DateTime cutoffDate = DateTime.Now.AddDays(-reportDays);
                List<string> categories = transactions.Where(t => (t.Date.CompareTo(cutoffDate) >= 0)).Select(t => t.Category).Distinct().ToList();


                /////
                // category summmaries - column showing gain/loss per category over X days
                if (ShowCategorySummaries)
                {
                    ws.Cells[1, 1, 1, 2].Merge = true;
                    ws.Cells[1, 1, 1, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[1, 1, 1, 2].Value = "Category Summaries";
                    ws.Cells[1, 1, 1, 2].Style.Font.Bold = true;
                    ws.Cells[2, 1].Value = "Category";
                    ws.Cells[2, 2].Value = "Gain/Loss";
                    ws.Cells[2, 1, 2, 2].Style.Font.Bold = true;



                    i = 3;
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
                }

                /////
                // balance graph - show balance per day over last X days
                if (ShowBalanceGraph)
                {
                    var bgCol = 1 + bgOffset;
                    var r = 10;
                    i = 1;
                    ws.Cells[r, bgCol].Value = currentBalance;
                    foreach (var t in transactions.Where(t => (t.Date.CompareTo(cutoffDate) >= 0)).ToList())
                    {
                        ws.Cells[r + i, bgCol].Value = t.Balance;
                        i++;
                    }
                }

                /////
                // show top 10 largest expenses in last X days
                if (ShowMostExpensivePurchases)
                {

                }

                /////
                // line graph sources of inflow over X days
                if (ReportInflowGraph)
                {

                }

                /////
                // line graph sources of outflow over X days
                if (ReportOutflowGraph)
                {

                }



                ws.Column(1).Width = 20;
                ws.Column(2).Width = 20;
                f.Save();                
            }
        }      
    }
}
