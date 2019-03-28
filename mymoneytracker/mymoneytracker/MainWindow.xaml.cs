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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<TransactionModel> saved;
        private List<RuleModel> rules;

        public MainWindow()
        {
            InitializeComponent();
            dpDate.SelectedDate = DateTime.Today;
            try
            {
                LoadTransactions();                
                Recent_Transactions.DataContext = this.saved;
                Rules_List.DataContext = this.rules;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured: " + ex.Message + "\n\nStack trace: " + ex.StackTrace, "Error!");
            }
        }

        private void LoadTransactions()
        {
            // get DB data
            var transactions = SqliteDataAccess.LoadTransactions();
            var rules = SqliteDataAccess.LoadRules();

            // apply categories to transactions
            Categorize.ApplyCategories(transactions, rules);

            this.saved = transactions;
            this.rules = rules;
        }

        private List<TransactionModel> GetTransactions()
        {
            // get DB data
            var transactions = SqliteDataAccess.LoadTransactions();
            var rules = SqliteDataAccess.LoadRules();

            // apply categories to transactions
            Categorize.ApplyCategories(transactions, rules);

            return transactions;
        }

        private void DeleteTransactionButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // todo: try just using LoadTransactions and dont delete from ui
                // remove from ui            
                int index = Recent_Transactions.SelectedIndex;
                if (index < 0 || index >= Recent_Transactions.Items.Count) {
                    return;
                }
                TransactionModel tm = Recent_Transactions.SelectedItem as TransactionModel;
                saved.RemoveAt(index);
                Recent_Transactions.Items.Refresh();

                // remove from db                            
                if (tm == null || tm.Id <= 0)
                {
                    return;
                }
                SqliteDataAccess.DeleteTransactionById(tm.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured: " + ex.Message + "\n\nStack trace: " + ex.StackTrace, "Error!");
            }
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
        private void BtnAddTransaction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // todo : handle empty/default values

                TransactionModel transaction = new TransactionModel();

                transaction.Date = dpDate.SelectedDate;
                transaction.Amount = Convert.ToDecimal(tbAmount.Text);
                transaction.Payee = tbPayee.Text;
                transaction.Category = tbCategory.Text;
                transaction.Custom_notes = tbNotes.Text;

                SqliteDataAccess.SaveTransaction(transaction);
                this.saved = GetTransactions();
                Recent_Transactions.DataContext = saved;

                dpDate.SelectedDate = DateTime.Today;
                tbAmount.Text = "Amount";
                tbPayee.Text = "Payee";
                tbCategory.Text = "Category";
                tbNotes.Text = "Notes";
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured: " + ex.Message + "\n\nStack trace: " + ex.StackTrace, "Error!");
            }
        }


        private void DeleteRuleButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // remove from ui            
                int index = Rules_List.SelectedIndex;
                if (index < 0 || index >= Rules_List.Items.Count)
                {
                    return;
                }
                RuleModel rule = Rules_List.SelectedItem as RuleModel;
                rules.RemoveAt(index);
                Rules_List.Items.Refresh();

                // remove from db                            
                if (rule == null)
                {
                    return;
                }
                SqliteDataAccess.DeleteRuleByName(rule.Rule_name);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured: " + ex.Message + "\n\nStack trace: " + ex.StackTrace, "Error!");
            }
        }

        private void BtnAddRule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RuleModel rule = new RuleModel();
                
                // todo : handle empty/default values
                rule.Rule_name = tbRuleName.Text;
                rule.Category = tbRuleCategory.Text;
                rule.Payee_regex = tbRuleMatchRegex.Text;
                // todo: inflow/outflow
                rule.Direction = tbDirection.Text;


                SqliteDataAccess.SaveRule(rule);
                this.rules = SqliteDataAccess.LoadRules();
                Rules_List.DataContext = this.rules;

                tbRuleName.Text = "";
                tbRuleCategory.Text = "";
                tbRuleMatchRegex.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured: " + ex.Message + "\n\nStack trace: " + ex.StackTrace, "Error!");
            }
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
    }
}
