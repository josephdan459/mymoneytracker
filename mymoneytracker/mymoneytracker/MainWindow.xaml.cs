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

        public MainWindow()
        {
            InitializeComponent();
            dpDate.SelectedDate = DateTime.Today;
            try
            {
                this.saved = SqliteDataAccess.LoadTransactions();
                Recent_Transactions.DataContext = saved;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured: " + ex.Message + "\n\nStack trace: " + ex.StackTrace, "Error!");
            }
        }

        private void DeleteTransactionButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = Recent_Transactions.SelectedIndex;
                TransactionModel tm = Recent_Transactions.SelectedItem as TransactionModel;

                // remove from ui            
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
        
        private void Recent_Transactions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
                TransactionModel transaction = new TransactionModel();

                transaction.Date = dpDate.SelectedDate;
                transaction.Amount = Convert.ToDecimal(tbAmount.Text);
                transaction.Payee = tbPayee.Text;
                transaction.Category = tbCategory.Text;
                transaction.Custom_notes = tbNotes.Text;

                SqliteDataAccess.SaveTransaction(transaction);
                this.saved = SqliteDataAccess.LoadTransactions();
                Recent_Transactions.DataContext = saved;

                dpDate.SelectedDate = DateTime.Today;
                tbAmount.Text = "";
                tbPayee.Text = "";
                tbCategory.Text = "";
                tbNotes.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured: " + ex.Message + "\n\nStack trace: " + ex.StackTrace, "Error!");
            }
        }
    }
}
