using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MblAPI.Model
{
    public class ItemSaleDayRpt
    {
        public string OuterIid { get; set; }
        public string OuterSkuId { get; set; }
        public DateTime PayTime { get; set; }
        public int Num { get; set; }
    }
}