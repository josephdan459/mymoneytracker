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
    class Reports
    {
        public static void CreateBasicReport(string reportPath, IEnumerable<TransactionModel> transactions, int reportDays, Decimal currentBalance, bool ShowCategorySummaries, bool ShowBalanceGraph, bool ShowMostExpensivePurchases, bool ReportInflowGraph, bool ReportOutflowGraph)
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

                    ws.Column(1).Width = 20;
                    ws.Column(2).Width = 20;

                    bgOffset += 3;
                }

                /////
                // balance graph - show balance per day over last X days
                if (ShowBalanceGraph && false)
                {
                    var bgCol = 1 + bgOffset;
                    var r = 10;
                    i = 1;
                    foreach (var t in transactions.Where(t => (t.Date.CompareTo(cutoffDate) >= 0)).ToList())
                    {
                        ws.Cells[r + i, bgCol].Value = i;
                        ws.Cells[r + i, bgCol + 1].Value = t.Balance;
                        i++;
                    }

                    ExcelChart lineChart = ws.Drawings.AddChart("lineChart", eChartType.Line);
                    lineChart.Title.Text = "Account Balance";

                    var balanceRange = ws.Cells[r + 1, bgCol, r + 1 + transactions.Where(t => (t.Date.CompareTo(cutoffDate) >= 0)).Count() - 1, bgCol];
                    var balanceAmounts = ws.Cells[r + 1, bgCol + 1, r + 1 + transactions.Where(t => (t.Date.CompareTo(cutoffDate) >= 0)).Count() - 1, bgCol + 1];
                    balanceRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    balanceAmounts.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    var bs = (ExcelLineChartSerie)lineChart.Series.Add(balanceAmounts, balanceRange);
                    //bs.Header = "Account Balance";

                    //size of the chart
                    lineChart.SetSize(600, 300);

                    //add the chart at cell B6
                    lineChart.SetPosition(5, 0, 1, 0);

                    bgOffset += 3;


                }

                /////
                // show top 10 largest expenses in last X days
                if (ShowMostExpensivePurchases)
                {
                    var bgCol = 3 + bgOffset;
                    var r = 4;

                    ws.Cells[r, bgCol, r, bgCol + 2].Merge = true;
                    ws.Cells[r, bgCol, r, bgCol + 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    ws.Cells[r, bgCol, r, bgCol + 2].Value = "Largest Expenses";
                    ws.Cells[r, bgCol, r, bgCol + 2].Style.Font.Bold = true;

                    r += 1;
                    ws.Cells[r, bgCol].Value = "Payee";
                    ws.Cells[r, bgCol + 1].Value = "Amount";
                    ws.Cells[r, bgCol + 2].Value = "Category";

                    List<TransactionModel> mostExpensive = transactions.Where(t => t.Amount < 0).OrderBy(t => t.Amount).Take(10).ToList();                    
                    i = 1;
                    foreach (var t in mostExpensive)
                    {
                        ws.Cells[r + i, bgCol].Value = t.Payee;
                        ws.Cells[r + i, bgCol + 1].Value = t.Amount;
                        ws.Cells[r + i, bgCol + 2].Value = t.Category;
                        i++;
                    }


                    ws.Column(bgCol).Width = 15;
                    ws.Column(bgCol + 1).Width = 15;
                    ws.Column(bgCol + 2).Width = 15;

                    bgOffset += 3;
                }

                /////
                // pie graph sources of inflow over X days
                if (ReportInflowGraph)
                {

                }

                /////
                // pie graph sources of outflow over X days
                if (ReportOutflowGraph)
                {

                }



                f.Save();
            }
        }
    }
}
