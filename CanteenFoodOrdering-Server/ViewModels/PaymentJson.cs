using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.ViewModels
{
    public class PaymentJson
    {
        public string public_key { get; set; }
        public string version { get; set; }
        public string action { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string description { get; set; }
        public string order_Id { get; set; }
    }
}
