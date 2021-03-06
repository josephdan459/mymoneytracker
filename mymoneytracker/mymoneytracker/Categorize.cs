﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace mymoneytracker
{
    public class Categorize
    {

        public static Decimal ApplyCategoriesAndBalances(ObservableCollection<TransactionModel> transactions, ObservableCollection<RuleModel> rules, Decimal balance)
        {
            // Iterate through each transaction and add categories for any that are not manually set
            foreach (var transaction in transactions.Reverse())
            {
                // show account balance at time of transaction
                balance += transaction.Amount;
                transaction.Balance = balance;                

                // apply category to transaction, only if not manually set
                // (Requirement 1.1.3)
                if (transaction.Category == null || transaction.Category == "")
                {
                    CategorizeTransaction(transaction, rules);
                }
            }

            return Math.Round(balance, 2);
        }

        private static void CategorizeTransaction(TransactionModel transaction, ObservableCollection<RuleModel> rules)
        {
            // Categorize transaction by trying each rule until one matches.
            foreach (var rule in rules) {
                if (TryCategorizeTransaction(transaction, rule)){
                    return;
                }
            }
        }

        private static bool TryCategorizeTransaction(TransactionModel transaction, RuleModel rule)
        {
            // Tests if this rule matches the transaction. Sets appropriate category if it does.

            // check in/out direction constraint 
            if ((rule.Direction == "Inflow" && transaction.Amount < 0) || (rule.Direction == "Outflow" && transaction.Amount > 0))
            {
                return false;
            }

            // check if rule matches
            var rx = new Regex(rule.Payee_regex);
            if (rx.IsMatch(transaction.Payee)){
                transaction.Category = rule.Category;
                return true;
            }
            return false;
        }
    }
}
