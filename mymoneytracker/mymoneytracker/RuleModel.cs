using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace mymoneytracker
{
    public class RuleModel : INotifyPropertyChanged, IDataErrorInfo
    {        
        public RuleModel()
        {
            Rule_name = "Rule Name";
            Category = "Category";
            Payee_regex = "Match Text";
            Direction = "Outflow";
        }
        #region Fields
        private string _rule_name;
        private string _category;
        private string _payee_regex;
        private string _direction;
        private string _error;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #region Properties
        public int Id { get; }
        public string Rule_name
        {
            get
            {
                return _rule_name;
            }
            set
            {
                if (value != _rule_name)
                {
                    _rule_name = value;
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
        public string Payee_regex
        {
            get
            {
                return _payee_regex;
            }
            set
            {
                if (value != _payee_regex)
                {
                    _payee_regex = value;
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
                if (columnName == "Rule_name")
                {
                    if (this.Rule_name.Length < 1)
                        result = "Rule name can not be empty.";
                    if (this.Rule_name.Length > 20)
                        result = "Rule name is too long.";
                    if (this.Rule_name == "Rule Name")
                        result = "Must provide rule name.";                    
                }
                if (columnName == "Category")
                {
                    if (this.Category.Length < 1)
                        result = "Category name can not be empty.";
                    if (this.Category.Length > 20)
                        result = "Category name is too long.";
                    if (this.Category == "Category")
                        result = "Must provide category name.";
                }
                if (columnName == "Payee_regex")
                {
                    if (this.Payee_regex.Length < 1)
                        result = "Payee name can not be empty.";
                    if (this.Payee_regex.Length > 20)
                        result = "Payee name is too long.";
                    if (this.Payee_regex == "Match Text")
                        result = "Must provide rule match text.";
                }
                if (columnName == "Direction")
                {
                    if (this.Direction.Length < 1 || this.Direction == "")
                        result = "Must choose direction.";                    
                }
                Error = result;
                return result;
            }
        }
        #endregion
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
