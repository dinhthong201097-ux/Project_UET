using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UII.Model
{
    public class PageModel
    {
        public int CustomerCount { get; set; }
        public string ProductStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TransactionValue { get; set; }
        public DateTime ShipmentDelivery { get; set; }
        public bool LocationStatus { get; set; }

    }
}
