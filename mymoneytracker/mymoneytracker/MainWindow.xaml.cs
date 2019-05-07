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

        public MainWindow()
        {
            InitializeComponent();
            Recent_Transactions.CellEditEnding += Transactions_CellEditEnding;

            viewModel = new ViewModels.MainViewModel();
            this.DataContext = viewModel;                        
        }

        // Save transaction from the user
        // (Requirement 1.1.0)

        private void BtnAddTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.NewTransaction.Date.CompareTo(System.DateTime.Now) > 0)
            {
                viewModel.NewTransaction.Date = System.DateTime.Now;
                errorContent.Content = "No future dates!";
                return;
            }
            if (viewModel.NewTransaction.Payee == "Payee")
            {
                errorContent.Content = "Must provide Payee!";
                return;
            }
            if (viewModel.NewTransaction.Amount == 0)
            {
                errorContent.Content = "Must provide Amount!";
                return;
            }
            if (viewModel.NewTransaction.Category == "Category")
            {
                viewModel.NewTransaction.Category = "";
            }
            if (viewModel.NewTransaction.Custom_notes == "Notes")
            {
                viewModel.NewTransaction.Custom_notes = "";
            }

            viewModel.AddTrans(cbDirectionTransaction.Text);

            // refresh current balance label
            BalanceLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();
            viewModel.NewTransaction.DefaultAll();
            errorContent.Content = "";
        }
        private void DeleteTransactionButtonClick(object sender, RoutedEventArgs e)
        {
            viewModel.RemoveTrans();
            // refresh current balance label
            BalanceLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();
        }

        // Save rule from the user
        // (Requirement 1.1.2)
        private void BtnAddRule_Click(object sender, RoutedEventArgs e)
        {
            
            if (viewModel.NewRule.Rule_name == "Rule Name")
            {
                ruleErrorContent.Content = "Must provide rule name!";
                return;
            }
            if (viewModel.NewRule.Category == "Category")
            {
                ruleErrorContent.Content = "Must provide rule category!";
                return;
            }
            if (viewModel.NewRule.Payee_regex == "Match Text")
            {
                ruleErrorContent.Content = "Must provide match text!";
                return;
            }

            viewModel.AddRule();
            viewModel.NewRule.DefaultAll();
            ruleErrorContent.Content = "";
        }
        private void DeleteRuleButtonClick(object sender, RoutedEventArgs e)
        {
            viewModel.RemoveRule();
        }
        private void TbAmount_GotFocus(object sender, RoutedEventArgs e)
        {
            if(tbAmount.Text == "$0.00")
                tbAmount.Text = "";
        }
        private void TbPayee_GotFocus(object sender, RoutedEventArgs e)
        {
            if(tbPayee.Text == "Payee")
                tbPayee.Text = "";
        }
        private void TbCategory_GotFocus(object sender, RoutedEventArgs e)
        {   
            if (tbCategory.Text == "Category")
                tbCategory.Text = "";
        }
        private void TbNotes_GotFocus(object sender, RoutedEventArgs e)
        {
            if(tbNotes.Text == "Notes")
                tbNotes.Text = "";
        }
        private void TbRuleName_GotFocus(object sender, RoutedEventArgs e)
        {
            if(tbRuleName.Text == "Rule Name")
                tbRuleName.Text = "";
        }
        private void TbRuleCategory_GotFocus(object sender, RoutedEventArgs e)
        {
            if(tbRuleCategory.Text == "Category")
                tbRuleCategory.Text = "";
        }
        private void TbRuleMatchRegex_GotFocus(object sender, RoutedEventArgs e)
        {
            if(tbRuleMatchRegex.Text == "Match Text")
                tbRuleMatchRegex.Text = "";
        }
        private void TransactionTab_LostFocus(object sender, RoutedEventArgs e)
        {
            viewModel.NewTransaction.DefaultAll();
            errorContent.Content = "";            
        }
        private void RuleTab_LostFocus(object sender, RoutedEventArgs e)
        {
            viewModel.NewRule.DefaultAll();
            ruleErrorContent.Content = "";           
        }

        // Save report to file now
        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            int reportDays;
            try
            {
                // (Requirement 2.2.5)
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
            dialog.DefaultExt = ".xlsx";
            dialog.Filter = "xlsx|*.xlsx";

            if (dialog.ShowDialog() == true)
            {
                string reportPath = dialog.FileName;
                string ss = viewModel.CurrentBalance.Replace("$", "");
                Decimal cb = Convert.ToDecimal(ss);
                // (Requirement 2.2.6)
                Reports.CreateBasicReport(reportPath, viewModel.Saved.OrderByDescending(t => t.Date), reportDays, cb, ShowCategorySummaries.IsChecked.Value, ShowMostExpensivePurchases.IsChecked.Value, ReportInflow.IsChecked.Value, ReportOutflow.IsChecked.Value);
            }
        }
        private void ImportCsvBtnClick(object sender, RoutedEventArgs e)
        {
            if (viewModel.Importing)
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

                try
                {
                    results = CsvImport.TransactionsFromCsv(csvPath, currentDate, dateCol, amountCol, payeeCol, db);
                }                
                catch
                {
                    // (Requirement 2.1.3)
                    CsvImportStatus.Content = "Import failed!";
                    return;
                }
            } else
            {
                // (Requirement 2.1.3)
                CsvImportStatus.Content = "Invalid import selection";
                return;
            }

            CsvImportStatus.Content = results.status;
            if (results.status == CsvImport.successStatus)
            {
                // show preview of first transaction
                // (Requirement 2.1.2)
                CsvDatePreview.Content = "Date: " + results.it.First().Date.ToString();
                CsvAmountPreview.Content = "Amount: " + results.it.First().Amount.ToString();
                CsvPayeePreview.Content = "Payee: " + results.it.First().Payee.ToString();
                CsvNumberPreview.Content = $"and {results.it.Count - 1} more...";
                viewModel.Importing = true;
                viewModel.Imported = results.it;
            }
        }    
        private void CancelImportClick(object sender, RoutedEventArgs e)
        {
            CsvImportStatus.Content = "Status:";
            CsvDatePreview.Content = "";
            CsvAmountPreview.Content = "";
            CsvPayeePreview.Content = "";
            CsvNumberPreview.Content = "";
            viewModel.Imported = null;
            viewModel.Importing = false;
        }
        private void ConfirmImportClick(object sender, RoutedEventArgs e)
        {

            if (!viewModel.Importing)
            {
                return;
            }

            CsvImportStatus.Content = $"Status: Processing {viewModel.Imported.Count} new transactions...";

            foreach (TransactionModel t in viewModel.Imported) {
                SqliteDataAccess.SaveTransaction(t);
            }
            viewModel.RefreshData();
            BalanceLabel.GetBindingExpression(Label.ContentProperty).UpdateTarget();

            CsvDatePreview.Content = "";
            CsvAmountPreview.Content = "";
            CsvPayeePreview.Content = "";
            CsvNumberPreview.Content = "";
            CsvImportStatus.Content = $"Status: {viewModel.Imported.Count} transactions added";
            viewModel.Imported = null;
            viewModel.Importing = false;           
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
                    try
                    {
                        switch (changedColumn)
                        {
                            case "Date":
                                DateTime d;
                                try
                                {
                                    d = Convert.ToDateTime(el.Text);
                                }
                                catch
                                {
                                    // error parsing
                                    this.Recent_Transactions.CancelEdit();
                                    return;
                                }
                               if (d.CompareTo(System.DateTime.Now) > 0){
                                    // cannot use a future date
                                    this.Recent_Transactions.CancelEdit();
                                    return;
                                }                                
                                editedTransaction.Date = d;
                                break;
                            case "Amount":
                                Decimal a;
                                try
                                {
                                    a = Convert.ToDecimal(el.Text.Replace("$", ""));
                                }
                                catch
                                {
                                    // error parsing
                                    this.Recent_Transactions.CancelEdit();
                                    return;
                                }
                                //Created bug; unable to maintain an outflow transaction on edit
                                //if (a <= 0)
                                //{
                                //    // amount cannot be negative
                                //    this.Recent_Transactions.CancelEdit();
                                //    return;
                                //}
                                editedTransaction.Amount = a;
                                break;
                            case "Payee":
                                editedTransaction.Payee = el.Text;
                                if (el.Text == "")
                                {
                                    // payee cannot be blank
                                    this.Recent_Transactions.CancelEdit();
                                    return;
                                }
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
                    catch
                    {
                        return;
                    }
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
                            if (el.Text == "")
                            {
                                Rules_List.CancelEdit();
                                return;
                            }
                            editedRule.Rule_name = el.Text;
                            break;
                        case "Category":
                            if (el.Text == "")
                            {
                                Rules_List.CancelEdit();
                                return;
                            }
                            editedRule.Category = el.Text;
                            break;
                        case "Payee_regex":
                            if (el.Text == "")
                            {
                                Rules_List.CancelEdit();
                                return;
                            }
                            editedRule.Payee_regex = el.Text;
                            break;
                        case "Direction":
                            if (el.Text != "Inflow" && el.Text != "Outflow")
                            {
                                Rules_List.CancelEdit();
                                return;
                            }
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
