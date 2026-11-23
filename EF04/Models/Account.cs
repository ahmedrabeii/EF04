using EF04.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF04.Models
{
    public class Account
    {
        public string AccountNumber { get; set; }

        public decimal CurrentBalance { get; set; }

        public AccountType AccountType { get; set; }

        public DateTime OpeningDate { get; set; }

        public string BranchCode { get; set; }

        public Branch Branch { get; set; }

        public ICollection<Transaction> Transactions { get; set; }

        public ICollection<CustomerAccount> CustomerAccounts { get; set; }
    }
}
