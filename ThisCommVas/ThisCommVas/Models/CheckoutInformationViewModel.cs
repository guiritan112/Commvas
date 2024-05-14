using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThisCommVas.Models
{
    public class CheckoutInformationViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PaymentMethod { get; set; }
        // Other properties related to checkout information
    }
}