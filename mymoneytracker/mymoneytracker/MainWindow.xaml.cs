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

            viewModel = new ViewModels.MainViewModel();
            this.DataContext = viewModel;           
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            dialog.Title = "Save report to...";
            dialog.DefaultExt = ".xls";
            dialog.Filter = "xls|*.xls";

            if (dialog.ShowDialog() == true)
            {
                string reportPath = dialog.FileName;
                Reports.CreateBasicReport(reportPath);
            }
        }
    }
}
