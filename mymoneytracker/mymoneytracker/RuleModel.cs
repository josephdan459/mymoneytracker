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
        }
        #region Fields
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        public int Id { get; }
        public string Rule_name { get; set; }
        public string Category { get; set; }
        public string Payee_regex { get; set; }        
        public string Direction { get; set; }
        public string Error
        {
            get { return null; }
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
                }
                if (columnName == "Category")
                {
                    if (this.Category.Length < 1)
                        result = "Category name can not be empty.";
                    if (this.Category.Length > 20)
                        result = "Category name is too long.";
                }
                if (columnName == "Payee_regex")
                {
                    if (this.Payee_regex.Length < 1)
                        result = "Payee name can not be empty.";
                    if (this.Payee_regex.Length > 20)
                        result = "Payee name is too long.";
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
