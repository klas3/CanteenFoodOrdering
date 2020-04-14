using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanteenFoodOrdering_Server.ViewModels
{
    public class PaymentJson
    {
        public string Public_Key { get; set; }
        public string Version { get; set; }
        public string Action { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string Order_Id { get; set; }
    }
}
