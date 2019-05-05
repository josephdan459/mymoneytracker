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
        private string _payee;
        private decimal _amount;
        private decimal _balance;
        private string _direction;
        private string _category;
        private string _custom_notes;
        private string _error;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region Properties
        public int Id { get; }
        public DateTime Date { get; set; }
        public string Payee
        {
            get
            {
                return _payee;
            }
            set
            {
                if (value != _payee)
                {
                    _payee = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public decimal Amount
        {
            get
            {
                return _amount;
            }
            set
            {
                if (value != _amount)
                {
                    _amount = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public decimal Balance
        {
            get
            {
                return _balance;
            }
            set
            {
                if (value != _balance)
                {
                    _balance = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                if (value != _direction)
                {
                    _direction = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                if (value != _category)
                {
                    _category = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Custom_notes
        {
            get
            {
                return _custom_notes;
            }
            set
            {
                if (value != _custom_notes)
                {
                    _custom_notes = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Error
        {
            get
            {
                return _error;
            }
            set
            {
                if (value != _error)
                {
                    _error = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;
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
                    if (this.Category.Length > 20)
                        result = "Category name is too long.";
                }
                if (columnName == "Custom_notes")
                {
                    if (this.Custom_notes.Length > 140)
                        result = "Category name is too long.";
                }
                Error = result;
                return result;
            }
        }
        #endregion
        #region Methods
        public void DefaultAll()
        {
            Date = DateTime.Today;
            Payee = "Payee";
            Amount = 0;
            Category = "Category";
            Direction = "Outflow";
            Custom_notes = "Notes";
            Error = "";
        }       
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}