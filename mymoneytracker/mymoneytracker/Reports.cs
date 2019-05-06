using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;

namespace mymoneytracker
{
    // Generates .xlsx reports from transaction information in the database    
    class Reports
    {
        public static void CreateBasicReport(string reportPath, IEnumerable<TransactionModel> transactions, int reportDays, Decimal currentBalance, bool ShowCategorySummaries, bool ShowMostExpensivePurchases, bool ReportInflow, bool ReportOutflow)
        {

            var bgOffset = 0;
            var i = 0;

            if (File.Exists(reportPath))
            {
                File.Delete(reportPath);
            }

            var fp = new FileInfo(reportPath);

            using (var f = new ExcelPackage(fp))
            {
                var ws = f.Workbook.Worksheets.Add($"My {reportDays}-day Budget Report");

                // extract useful statistics from transaction data
                // (Requirement 1.1.5)
                DateTime cutoffDate = DateTime.Now.AddDays(-reportDays);
                List<string> categories = transactions.Where(t => (t.Date.CompareTo(cutoffDate) >= 0)).Select(t => t.Category).Distinct().ToList();
                List<string> inflowSources = transactions.Where(t => ((t.Date.CompareTo(cutoffDate) >= 0) && (t.Amount > 0))).Select(t => t.Payee).Distinct().ToList();
                List<string> outflowSources = transactions.Where(t => ((t.Date.CompareTo(cutoffDate) >= 0) && (t.Amount < 0))).Select(t => t.Payee).Distinct().ToList();
                
                /////
                // category summmaries - column showing gain/loss per category over X days
                // (Requirement 2.2.1)
                if (ShowCategorySummaries)
                {
                    // format cells
                    ws.Cells[1, 1, 1, 2].Merge = true;
                    ws.Cells[1, 1, 1, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[1, 1, 1, 2].Value = "Category Summaries";
                    ws.Cells[1, 1, 1, 2].Style.Font.Bold = true;
                    ws.Cells[2, 1].Value = "Category";
                    ws.Cells[2, 2].Value = "Gain/Loss";
                    ws.Cells[2, 1, 2, 2].Style.Font.Bold = true;

                    // write gain/loss per category
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
                    ws.Column(1).Width = 20;
                    ws.Column(2).Width = 20;
                    bgOffset += 3;
                }

                /////
                // show top 10 largest expenses in last X days
                // (Requirement 2.2.2)
                if (ShowMostExpensivePurchases)
                {
                    // format cells
                    var bgCol = 1 + bgOffset;
                    var r = 1;
                    ws.Cells[r, bgCol, r, bgCol + 2].Merge = true;
                    ws.Cells[r, bgCol, r, bgCol + 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[r, bgCol, r, bgCol + 2].Value = "Largest Expenses";
                    ws.Cells[r, bgCol, r, bgCol + 2].Style.Font.Bold = true;
                    r += 1;
                    ws.Cells[r, bgCol].Value = "Payee";
                    ws.Cells[r, bgCol + 1].Value = "Amount";
                    ws.Cells[r, bgCol + 2].Value = "Category";
                    ws.Column(bgCol).Width = 25;
                    ws.Column(bgCol + 1).Width = 15;
                    ws.Column(bgCol + 2).Width = 20;

                    // write 10 largest transactions
                    List<TransactionModel> mostExpensive = transactions.Where(t => t.Amount < 0 && (t.Date.CompareTo(cutoffDate) >= 0)).OrderBy(t => t.Amount).Take(10).ToList();                    
                    i = 1;
                    foreach (var t in mostExpensive)
                    {
                        ws.Cells[r + i, bgCol].Value = t.Payee;
                        ws.Cells[r + i, bgCol + 1].Value = t.Amount;
                        ws.Cells[r + i, bgCol + 2].Value = t.Category;
                        i++;
                    }                    
                    bgOffset += 4;
                }


                /////
                // show sources of inflow over X days
                // (Requirement 2.2.3)
                if (ReportInflow)
                {
                    // format cells
                    var bgCol = 1 + bgOffset;
                    var r = 1;
                    ws.Cells[r, bgCol, r, bgCol + 1].Merge = true;
                    ws.Cells[r, bgCol, r, bgCol + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[r, bgCol, r, bgCol + 1].Value = "Sources of Inflow";
                    ws.Cells[r, bgCol, r, bgCol + 1].Style.Font.Bold = true;

                    // write gain per payee
                    var gains = new List<Tuple<decimal, string>> { };                    
                    foreach (var p in inflowSources)
                    {
                        // sum the changes over last X days                    
                        decimal changed = (from t in transactions where ((t.Payee == p) && (t.Date.CompareTo(cutoffDate) >= 0) && (t.Amount > 0)) select t.Amount).Sum();

                        gains.Add(Tuple.Create(changed, p));
                    }

                    // sort and print totals
                    i = 3;
                    foreach (var g in gains.OrderByDescending(g => g.Item1))
                    {
                        ws.Cells[i, bgCol].Value = g.Item2;
                        ws.Cells[i, bgCol + 1].Value = g.Item1;
                        i++;
                    }
                    ws.Column(bgCol).Width = 25;
                    ws.Column(bgCol+1).Width = 20;
                    bgOffset += 3;
                }

                /////
                // show sources of outflow over X days
                // (Requirement 2.2.4)
                if (ReportOutflow)
                {
                    // format cells
                    var bgCol = 1 + bgOffset;
                    var r = 1;
                    ws.Cells[r, bgCol, r, bgCol + 1].Merge = true;
                    ws.Cells[r, bgCol, r, bgCol + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[r, bgCol, r, bgCol + 1].Value = "Sources of Outflow";
                    ws.Cells[r, bgCol, r, bgCol + 1].Style.Font.Bold = true;

                    // write loss per payee
                    var losses = new List<Tuple<decimal, string>> { };
                    foreach (var p in outflowSources)
                    {
                        // sum the changes over last X days                    
                        decimal changed = (from t in transactions where ((t.Payee == p) && (t.Date.CompareTo(cutoffDate) >= 0) && (t.Amount < 0)) select t.Amount).Sum();

                        losses.Add(Tuple.Create(changed, p));
                    }

                    // sort and print totals
                    i = 3;
                    foreach (var l in losses.OrderBy(l => l.Item1))
                    {
                        ws.Cells[i, bgCol].Value = l.Item2;
                        ws.Cells[i, bgCol + 1].Value = l.Item1;
                        i++;
                    }

                    ws.Column(bgCol).Width = 25;
                    ws.Column(bgCol + 1).Width = 20;
                    bgOffset += 3;
                }

                f.Save();
            }
        }
    }
}
