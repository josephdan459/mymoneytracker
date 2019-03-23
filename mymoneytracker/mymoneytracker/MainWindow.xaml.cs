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

            this.saved = SqliteDataAccess.LoadTransactions();
            Recent_Transactions.DataContext = saved;
        }

        private void DeleteTransactionButtonClick(object sender, RoutedEventArgs e)
        {
            int index = Recent_Transactions.SelectedIndex;
            TransactionModel tm = Recent_Transactions.SelectedItem as TransactionModel;

            // remove from ui            
            saved.RemoveAt(index);
            Recent_Transactions.Items.Refresh();

            // remove from db            
            if (tm == null || tm.Id <= 0) {
                return;
            }
            SqliteDataAccess.DeleteTransactionById(tm.Id);
        }
        
        private void Recent_Transactions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
