using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MblAPI.Model
{
    public class UserSaleDayRpt
    {
        public string UserName { get; set; }

        public string SellerNick { get;set; }

        public DateTime PayTime { get; set; }

        public decimal Payment { get; set; }

        public decimal RefundFee { get; set; }
    }
}