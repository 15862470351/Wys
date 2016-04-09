using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MblAPI.Model
{
    public class UserSaleMonthRpt
    {
        public string UserName { get; set; }

        public string SellerNick { get; set; }

        public int Y { get; set; }

        public int M { get; set; }

        public decimal Payment { get; set; }

        public decimal RefundFee { get; set; }
    }
}