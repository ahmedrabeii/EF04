using EF04.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF04.Models
{
    public class CustomerAccount
    {
        public int CustomerId { get; set; }

        public string AccountNumber { get; set; }

        public OwnershipType OwnershipType { get; set; }

        public DateTime OwnershipStartDate { get; set; }

        public AccountStatus AccountStatus { get; set; }

        public Customer Customer { get; set; }

        public Account Account { get; set; }
    }
}
