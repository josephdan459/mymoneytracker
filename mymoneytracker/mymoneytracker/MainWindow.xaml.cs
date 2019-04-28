using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace mymoneytracker
{
    public partial class MainWindow : Window
    {
        private static ViewModels.MainViewModel viewModel;
        private bool importing;

        private List<TransactionModel> imported;

        public MainWindow()
        {
            InitializeComponent();

            Recent_Transactions.CellEditEnding += Transactions_CellEditEnding;

            viewModel = new ViewModels.MainViewModel();
            this.DataContext = viewModel;
            this.importing = false;
            this.imported = null;
        }

        private void BtnAddTransaction_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddTrans(cbDirectionTransaction.Text);
            // refresh current balance label
            BalanceLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();
        }
        private void DeleteTransactionButtonClick(object sender, RoutedEventArgs e)
        {
            viewModel.RemoveTrans();
            // refresh current balance label
            BalanceLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();
        }

        private void BtnAddRule_Click(object sender, RoutedEventArgs e)
        {
            viewModel.AddRule();
        }
        private void DeleteRuleButtonClick(object sender, RoutedEventArgs e)
        {
            viewModel.RemoveRule();
        }

        private void TbAmount_GotFocus(object sender, RoutedEventArgs e)
        {
            tbAmount.Text = "";
        }
        private void TbPayee_GotFocus(object sender, RoutedEventArgs e)
        {
            tbPayee.Text = "";
        }
        private void TbCategory_GotFocus(object sender, RoutedEventArgs e)
        {
            tbCategory.Text = "";
        }
        private void TbNotes_GotFocus(object sender, RoutedEventArgs e)
        {
            tbNotes.Text = "";
        }
        private void TbRuleName_GotFocus(object sender, RoutedEventArgs e)
        {
            tbRuleName.Text = "";
        }
        private void TbRuleCategory_GotFocus(object sender, RoutedEventArgs e)
        {
            tbRuleCategory.Text = "";
        }
        private void TbRuleMatchRegex_GotFocus(object sender, RoutedEventArgs e)
        {
            tbRuleMatchRegex.Text = "";
        }

        private void TransactionTab_LostFocus(object sender, RoutedEventArgs e)
        {
            tbAmount.Text = "0";
            tbPayee.Text = "Payee";
            tbCategory.Text = "Category";
            tbNotes.Text = "Notes";
        }

        private void RuleTab_LostFocus(object sender, RoutedEventArgs e)
        {
            tbRuleName.Text = "Rule Name";
            tbRuleMatchRegex.Text = "Match Text";
            tbRuleCategory.Text = "Category";
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            int reportDays;
            try
            {
                reportDays = Convert.ToInt32(reportWindow.Text);
                if (reportDays < 2) {
                    throw new Exception("");
                }
            }
            catch
            {
                // invalid user input, reset to good value
                reportWindow.Text = "30";
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            dialog.Title = "Save report to...";
            dialog.DefaultExt = ".xls";
            dialog.Filter = "xls|*.xls";

            if (dialog.ShowDialog() == true)
            {
                string reportPath = dialog.FileName;
                string ss = viewModel.CurrentBalance.Replace("$", "");
                Decimal cb = Convert.ToDecimal(ss);
                Reports.CreateBasicReport(reportPath, viewModel.Saved.OrderByDescending(t => t.Date), reportDays, cb, ShowCategorySummaries.IsChecked.Value, ShowMostExpensivePurchases.IsChecked.Value, ReportInflow.IsChecked.Value, ReportOutflow.IsChecked.Value);
            }
        }


        private void ImportCsvBtnClick(object sender, RoutedEventArgs e)
        {
            if (this.importing)
            {
                return;
            }

            // make sure each column is selected
            int dateCol = -1;
            int amountCol;
            int payeeCol;
            string directionCol;
            DateTime? currentDate = null;
            try
            {
                ComboBoxItem i;

                if (ImportCsvDateToday.IsChecked.Value == true)
                {
                    currentDate = System.DateTime.Now;
                } else
                {
                    i = (ComboBoxItem)ImportCsvDateCol.SelectedItem;
                    dateCol = Convert.ToInt32(i.Content.ToString()) - 1;
                }

                i = (ComboBoxItem)ImportCsvAmountCol.SelectedItem;
                amountCol = Convert.ToInt32(i.Content.ToString()) - 1;

                i = (ComboBoxItem)ImportCsvPayeeCol.SelectedItem;
                payeeCol = Convert.ToInt32(i.Content.ToString()) - 1;

                i = (ComboBoxItem)ImportCsvDirection.SelectedItem;
                directionCol = i.Content.ToString();

                if ((ImportCsvDateToday.IsChecked.Value == false && dateCol == -1))
                {
                    CsvImportStatus.Content = "Select all columns first";
                    return;
                }
            }
            catch
            {
                CsvImportStatus.Content = "Select all columns first";
                return;
            }

            // open filepicker to csv file
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            dialog.Title = "Import transactions from...";
            dialog.DefaultExt = ".csv";
            dialog.Filter = "csv|*.csv";

            CsvImport results;
            results = new CsvImport();
            if (dialog.ShowDialog() == true)
            {
                string csvPath = dialog.FileName;

                CsvImport.DirectionBehavior db;
                if (directionCol == "Inflow")
                {
                    db = CsvImport.DirectionBehavior.Inflow;
                } else if (directionCol == "Outflow")
                {
                    db = CsvImport.DirectionBehavior.Outflow;
                } else
                {
                    db = CsvImport.DirectionBehavior.Both;
                }

                results = CsvImport.TransactionsFromCsv(csvPath, currentDate, dateCol, amountCol, payeeCol, db);
            } else
            {
                CsvImportStatus.Content = "Invalid import selection";
                return;
            }

            CsvImportStatus.Content = results.status;
            if (results.status == CsvImport.successStatus)
            {
                // show preview of first transaction
                CsvDatePreview.Content = "Date: " + results.it.First().Date.ToString();
                CsvAmountPreview.Content = "Amount: " + results.it.First().Amount.ToString();
                CsvPayeePreview.Content = "Payee: " + results.it.First().Payee.ToString();
                CsvNumberPreview.Content = $"and {results.it.Count - 1} more...";
                this.importing = true;
                this.imported = results.it;
            }
        }

        private void CancelImportClick(object sender, RoutedEventArgs e)
        {
            CsvImportStatus.Content = "Status:";
            CsvDatePreview.Content = "";
            CsvAmountPreview.Content = "";
            CsvPayeePreview.Content = "";
            CsvNumberPreview.Content = "";
            this.imported = null;
            this.importing = false;
        }

        private void ConfirmImportClick(object sender, RoutedEventArgs e)
        {

            if (!this.importing)
            {
                return;
            }

            CsvImportStatus.Content = $"Status: Processing {this.imported.Count} new transactions...";

            foreach (TransactionModel t in this.imported) {
                SqliteDataAccess.SaveTransaction(t);
            }
            viewModel.RefreshData();
            BalanceLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();

            CsvDatePreview.Content = "";
            CsvAmountPreview.Content = "";
            CsvPayeePreview.Content = "";
            CsvNumberPreview.Content = "";
            CsvImportStatus.Content = $"Status: {this.imported.Count} transactions added";
            this.imported = null;
            this.importing = false;           
        }

        // Updates DB when transaction cells are edited by user
        private void Transactions_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // Need to get the new user-edited value with a switch statement
                var c = e.Column as DataGridBoundColumn;
                if (c != null)
                {
                    TransactionModel editedTransaction = (TransactionModel)e.Row.DataContext;
                    int rowIndex = e.Row.GetIndex();
                    var el = e.EditingElement as TextBox;
                    var changedColumn = (c.Binding as Binding).Path.Path;
                    switch (changedColumn)
                    {
                        case "Date":
                            // todo: verify valid date?
                            editedTransaction.Date = Convert.ToDateTime(el.Text);
                            break;
                        case "Amount":
                            editedTransaction.Amount = Convert.ToDecimal(el.Text.Replace("$", ""));
                            break;
                        case "Payee":
                            editedTransaction.Payee = el.Text;
                            break;
                        case "Category":
                            editedTransaction.Category = el.Text;
                            break;
                        case "Custom_notes":
                            editedTransaction.Custom_notes = el.Text;
                            break;
                        default:
                            // not allowed to change balance, return now
                            return;
                    }
                    // save edited transaction to DB and refresh UI
                    viewModel.UpdateTransaction(editedTransaction);
                    BalanceLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();
                }                
                
            }
        }

        private void Rules_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // Need to get the new user-edited value with a switch statement
                var c = e.Column as DataGridBoundColumn;
                if (c != null)
                {
                    RuleModel editedRule = (RuleModel)e.Row.DataContext;
                    int rowIndex = e.Row.GetIndex();
                    var el = e.EditingElement as TextBox;
                    var changedColumn = (c.Binding as Binding).Path.Path;
                    switch (changedColumn)
                    {
                        case "Rule_name":
                            editedRule.Rule_name = el.Text;
                            break;
                        case "Category":
                            editedRule.Category = el.Text;
                            break;
                        case "Payee_regex":
                            editedRule.Payee_regex = el.Text;
                            break;
                        case "Direction":
                            editedRule.Direction = el.Text;
                            break;
                        default:
                            return;
                    }
                    // save edited transaction to DB and refresh UI
                    viewModel.UpdateRule(editedRule);
                }

            }
        }
    }
}
