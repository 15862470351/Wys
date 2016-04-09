using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace MblAPI.Model
{
    public class RptSales
    {
       
        //public string SellerNick { get; set; }
        // [JsonProperty("日期")]
        //public DateTime Date { get; set; }

        //public decimal SalesVolume { get; set; }

        //public decimal refund { get; set; }

        public string 店铺 { get; set; }

        public DateTime 日期 { get; set;}

        public decimal 销售额 { get; set; }

        public decimal 退款 { get; set; }
    }
}