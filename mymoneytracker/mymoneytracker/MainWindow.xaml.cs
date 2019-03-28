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
                RefreshUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured: " + ex.Message + "\n\nStack trace: " + ex.StackTrace, "Error!");
            }
        }

        private void RefreshUI()
        {
            // get DB data
            this.saved  = SqliteDataAccess.LoadTransactions();
            this.rules = SqliteDataAccess.LoadRules();

            // apply current rules to current transactions
            Categorize.ApplyCategories(saved, rules);

            // refresh UI grids
            Recent_Transactions.DataContext = this.saved;
            Rules_List.DataContext = this.rules;
        }

        private void DeleteTransactionButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                TransactionModel tm = Recent_Transactions.SelectedItem as TransactionModel;
                if (tm == null || tm.Id <= 0)
                {
                    return;
                }
                SqliteDataAccess.DeleteTransactionById(tm.Id);
                RefreshUI();
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
                RefreshUI();

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
                RuleModel rule = Rules_List.SelectedItem as RuleModel;
                if (rule == null)
                {
                    return;
                }
                SqliteDataAccess.DeleteRuleByName(rule.Rule_name);
                RefreshUI();
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
                rule.Direction = tbDirection.Text;


                SqliteDataAccess.SaveRule(rule);
                RefreshUI();

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
