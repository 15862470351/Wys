using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MblAPI.Model
{
    public class ItemSaleMonthRpt
    {
        public string OuterIid { get; set; }
        public string OuterSkuId { get; set; }
        public int Num { get; set; }
        public int Y { get; set; }
        public int M { get; set; }
    }
}