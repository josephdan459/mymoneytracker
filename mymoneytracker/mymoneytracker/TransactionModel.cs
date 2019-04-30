using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace mymoneytracker
{
    
    public class TransactionModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public TransactionModel()
        {
            DefaultAll();
        }

        #region Fields
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public int Id { get; }
        public DateTime Date { get; set; }
        public string Payee { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string Category { get; set; }
        public string Custom_notes { get; set; }

        public string Error
        {
            get { return null; }
        }

        public void DefaultAll()
        {
            Date = DateTime.Today;
            Payee = "Payee";
            Amount = 0;
            Category = "Category";
            Custom_notes = "Notes";
        }
        
        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;
                if (columnName == "Date")
                {
                    if (this.Date > DateTime.Now)
                        result = "Transaction Date can not be a future date.";
                }                
                if (columnName == "Amount")
                {
                    if (this.Amount < 0)
                        result = "Amount can not be less than 0.";
                }
                if (columnName == "Payee")
                {
                    if (this.Payee.Length < 1)
                        result = "Payee name can not be empty.";
                    if (this.Payee.Length > 20)
                        result = "Payee name is too long.";
                }
                if (columnName == "Category")
                {
                    if (this.Category.Length < 1)
                        result = "Category name can not be empty.";
                    if (this.Category.Length > 20)
                        result = "Category name is too long.";
                }
                if (columnName == "Custom_notes")
                {                    
                    if (this.Custom_notes.Length > 140)
                        result = "Category name is too long.";
                }

                return result;
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}