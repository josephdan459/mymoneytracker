using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace mymoneytracker.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged, IDataErrorInfo
    {        
        public MainViewModel()
        {
            Importing = false;
            NewTransaction = new TransactionModel();
            NewRule = new RuleModel();
            Saved = new ObservableCollection<TransactionModel>();
            Rules = new ObservableCollection<RuleModel>();
            Imported = null;

            var StartingBalance = SqliteDataAccess.GetStartingBalance();
            if (StartingBalance == Decimal.MinValue)
            {
                var set = false;
                var err = "";

                while (!set)
                {
                    StartingBalanceDialog dialog = new StartingBalanceDialog();

                    if (err != "")
                    {
                        dialog.sbprompttext.Text = $"Input starting balance ({err})";
                    }

                    if (dialog.ShowDialog() == true && dialog.DialogResult == true)
                    {
                        Decimal sb;
                        try
                        {
                            sb = Convert.ToDecimal(dialog.ResponseText);
                        }
                        catch
                        {
                            err = "invalid input";
                            continue;
                        }

                        SqliteDataAccess.SetStartingBalance(sb);
                        set = true;
                    }
                    else
                    {
                        set = true;
                        System.Windows.Application.Current.Shutdown();
                    }
                }                
            }

            RefreshData();
        }
        #region Fields
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public bool Importing { get; set; }
        public string ErrorMessage { get; set; }

        public string Error
        {
            get { return null; }
        }

        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;
                /*Enter Property Name Below and Set Validation Condition;
                Refer to class definition file for referenced class files 
                for validation on properties within referenced classes
                if (columnName == "")
                {
                    if (this.Name == "")
                        result = "Name can not be empty";
                }
                */
                return result;
            }
        }

        public TransactionModel NewTransaction { get; set; }

        public TransactionModel SelectedTransaction { get; set; }

        public RuleModel NewRule { get; set; }

        public RuleModel SelectedRule { get; set; }

        public String CurrentBalance { get; set; }

        public ObservableCollection<TransactionModel> Saved { get; set; }

        public ObservableCollection<RuleModel> Rules { get; set; }

        public List<TransactionModel> Imported { get; set; }
        #endregion

        #region Methods
        public void AddTrans(string TransactionDirection)
        {
            try
            {
                if (NewTransaction.Error == null || NewTransaction.Error == "")
                {
                    if (TransactionDirection.Contains("Outflow"))
                    {
                        NewTransaction.Amount = -Math.Abs(NewTransaction.Amount);
                    }
                    else
                    {
                        NewTransaction.Amount = Math.Abs(NewTransaction.Amount);
                    }

                    SqliteDataAccess.SaveTransaction(NewTransaction);

                    RefreshData();
                }                
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
                if (NewRule.Error == null || NewRule.Error == "")
                {
                    if (NewRule.Direction == null)
                    {
                        throw new ArgumentException("Must select rule direction");
                    }

                    SqliteDataAccess.SaveRule(NewRule);

                    RefreshData();
                }                
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

        public void UpdateTransaction(TransactionModel editedTransaction)
        {
            try
            {
                SqliteDataAccess.UpdateTransaction(editedTransaction);
                RefreshData();
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occured: " + ex.Message + "\n\nStack trace: " + ex.StackTrace + "Error!";
            }
        }

        public void UpdateRule(RuleModel editedRule)
        {
            try
            {
                SqliteDataAccess.UpdateRule(editedRule);
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
            NewRule.DefaultAll();

            foreach (var row in SqliteDataAccess.LoadTransactions())
            {
                Saved.Add(row);
            }

            foreach (var row in SqliteDataAccess.LoadRules())
            {
                Rules.Add(row);
            }

            // apply current rules to current transactions and calculate account balance
            var cb = Categorize.ApplyCategoriesAndBalances(Saved, Rules, SqliteDataAccess.GetStartingBalance());            
            CurrentBalance = "$" + cb.ToString();
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
