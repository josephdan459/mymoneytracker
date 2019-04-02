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
            viewModel.AddTrans();
        }
        private void DeleteTransactionButtonClick(object sender, RoutedEventArgs e)
        {
            viewModel.RemoveTrans();
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

    }
}
