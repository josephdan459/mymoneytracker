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
        public MainWindow()
        {
            InitializeComponent();
            
            TransactionModel t1 = new TransactionModel();
            t1.Date = DateTime.Now.ToFileTime().ToString();
            t1.Payee = DateTime.Now.ToFileTime().ToString();

            SqliteDataAccess.SaveTransaction(t1);

            var saved = SqliteDataAccess.LoadTransactions();
            Console.WriteLine($"got saved with len {saved.Count}");
        }

        private void Recent_Transactions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
