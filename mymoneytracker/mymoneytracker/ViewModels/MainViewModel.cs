﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace mymoneytracker.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {        
        public MainViewModel()
        {
            NewTransaction = new TransactionModel();
            NewRule = new RuleModel();
            Saved = new ObservableCollection<TransactionModel>();
            Rules = new ObservableCollection<RuleModel>();

            var StartingBalance = SqliteDataAccess.GetStartingBalance();
            if (StartingBalance == Decimal.MinValue)
            {
                StartingBalanceDialog dialog = new StartingBalanceDialog();
                if (dialog.ShowDialog() == true && dialog.DialogResult == true)
                {
                    // todo: simple validation
                    SqliteDataAccess.SetStartingBalance(Convert.ToDecimal(dialog.ResponseText));
                } else
                {
                    ErrorMessage = "Must input starting balance!";
                    System.Windows.Application.Current.Shutdown();
                    return;
                }

            }

            RefreshData();
        }
        #region Fields
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public string ErrorMessage { get; set; }

        public TransactionModel NewTransaction { get; set; }

        public TransactionModel SelectedTransaction { get; set; }

        public RuleModel NewRule { get; set; }

        public RuleModel SelectedRule { get; set; }

        public Decimal CurrentBalance { get; set; }

        public ObservableCollection<TransactionModel> Saved { get; set; }

        public ObservableCollection<RuleModel> Rules { get; set; }
        #endregion

        #region Methods
        public void AddTrans()
        {
            try
            {
                // todo : handle empty/default values
                SqliteDataAccess.SaveTransaction(NewTransaction);                
                RefreshData();
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occured: " + ex.Message + "\n\nStack trace: " + ex.StackTrace + "Error!";
            }
        }

        public void RemoveTrans()
        {
            try
            {                
                SqliteDataAccess.DeleteTransactionById(SelectedTransaction.Id);
                RefreshData();
            }
            catch (Exception ex)
            {
                ErrorMessage=("An error occured: " + ex.Message + "\n\nStack trace: " + ex.StackTrace + "Error!");
            }
        }

        public void AddRule()
        {
            try
            {                
                SqliteDataAccess.SaveRule(NewRule);
                RefreshData();                
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occured: " + ex.Message + "\n\nStack trace: " + ex.StackTrace + "Error!";
            }
        }

        public void RemoveRule()
        {
            try
            {                
                SqliteDataAccess.DeleteRuleByName(SelectedRule.Rule_name);
                RefreshData();
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occured: " + ex.Message + "\n\nStack trace: " + ex.StackTrace + "Error!";
            }
        }

        public void RefreshData()
        {
            Saved.Clear();
            Rules.Clear();
            NewTransaction.DefaultAll();

            foreach (var row in SqliteDataAccess.LoadTransactions())
            {
                Saved.Add(row);
            }

            foreach (var row in SqliteDataAccess.LoadRules())
            {
                Rules.Add(row);
            }

            // apply current rules to current transactions and calculate account balance
            CurrentBalance = Categorize.ApplyCategoriesAndBalances(Saved, Rules, SqliteDataAccess.GetStartingBalance());
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
