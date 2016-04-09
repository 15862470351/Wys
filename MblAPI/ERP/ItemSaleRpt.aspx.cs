using MblAPI.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ultra.Web.Core.Common;

namespace MblAPI.ERP
{
    public partial class ItemSaleRpt : BasePage
    {
        //商品销量 日销量 月销量
        protected void Page_Load(object sender, EventArgs e)
        {
            var num = Request["num"];
            var time = Request["time"];
            ResultData rd = new ResultData { IsOK = false };
            var cnn = string.Empty;
            if (!CheckLogin())
            {
                rd.ErrMsg = "时间间隔超过24小时,请重新登录!";
            }
            else
            {
                if (!CheckDbCnn(out cnn) || !CheckMobile() || string.IsNullOrEmpty(num))
                {
                    rd.ErrMsg = "无效参数";
                }
                else
                {
                    #region
                    if (num == "1")//日销量
                    {
                        if (string.IsNullOrEmpty(time))
                        {
                            var dt = SqlHelper.ExecuteDataTable(cnn, CommandType.Text, @"select OuterIid,OuterSkuId,PayTime, SUM(Num)Num from V_ERP_NZPItemSaleNum 
 where DATEPART(YEAR,PayTime)=@0 and  DATEPART(Day,PayTime)=@1 
 group by OuterIid,OuterSkuId,PayTime  order by SUM(Num) desc ",
                       new SqlParameter("@0", DateTime.Now.Year), new SqlParameter("@1", DateTime.Now.Day));
                            if (dt == null || dt.Rows.Count < 1)
                            {
                                rd.ErrMsg = "无有效数据";
                            }
                            else
                            {
                                rd.IsOK = true;
                                rd.Data = Ultra.Web.Core.Common.ObjectHelper.Create<ItemSaleDayRpt>(dt);
                            }
                        }
                        else
                        {
                            DateTime ti;
                            if (!DateTime.TryParse(time, out ti))
                            {
                                rd.ErrMsg = "日期参数无效";
                            }
                            else
                            {
                                var dt = SqlHelper.ExecuteDataTable(cnn, CommandType.Text, @"select OuterIid,OuterSkuId,PayTime, SUM(Num)Num from V_ERP_NZPItemSaleNum 
 where DATEPART(YEAR,PayTime)=@0 and  DATEPART(Day,PayTime)=@1 
 group by OuterIid,OuterSkuId,PayTime  order by SUM(Num) desc ",
                              new SqlParameter("@0", ti.Year), new SqlParameter("@1", ti.Day));
                                if (dt == null || dt.Rows.Count < 1)
                                {
                                    rd.ErrMsg = "无有效数据";
                                }
                                else
                                {
                                    rd.IsOK = true;
                                    rd.Data = Ultra.Web.Core.Common.ObjectHelper.Create<ItemSaleDayRpt>(dt);
                                }
                            }
                        }
                    }
                    else if (num == "2")//月销量
                    {
                        if (string.IsNullOrEmpty(time))
                        {
                            var dt = SqlHelper.ExecuteDataTable(cnn, CommandType.Text, @" select OuterIid,OuterSkuId,YEAR(PayTime)Y,MONTH(PayTime)M,SUM(Num)Num from V_ERP_NZPItemSaleNum 
 where DATEPART(YEAR,PayTime)=@0 and  DATEPART(MONTH,PayTime)=@1 
 group by OuterIid,OuterSkuId,YEAR(PayTime),MONTH(PayTime) order by SUM(Num) desc",
                              new SqlParameter("@0", DateTime.Now.Year), new SqlParameter("@1", DateTime.Now.Month));
                            if (dt == null || dt.Rows.Count < 1)
                            {
                                rd.ErrMsg = "无有效数据";
                            }
                            else
                            {
                                rd.IsOK = true;
                                rd.Data = Ultra.Web.Core.Common.ObjectHelper.Create<ItemSaleMonthRpt>(dt);
                            }
                        }
                        else
                        {
                            DateTime ti;
                            if (!DateTime.TryParse(time, out ti))
                            {
                                rd.ErrMsg = "日期参数无效";
                            }
                            else
                            {
                                var dt = SqlHelper.ExecuteDataTable(cnn, CommandType.Text, @"  select OuterIid,OuterSkuId,YEAR(PayTime)Y,MONTH(PayTime)M,SUM(Num)Num from V_ERP_NZPItemSaleNum 
 where DATEPART(YEAR,PayTime)=@0 and  DATEPART(MONTH,PayTime)=@1 
 group by OuterIid,OuterSkuId,YEAR(PayTime),MONTH(PayTime) order by SUM(Num) desc",
                                  new SqlParameter("@0", ti.Year), new SqlParameter("@1", ti.Month));
                                if (dt == null || dt.Rows.Count < 1)
                                {
                                    rd.ErrMsg = "无有效数据";
                                }
                                else
                                {
                                    rd.IsOK = true;
                                    rd.Data = Ultra.Web.Core.Common.ObjectHelper.Create<ItemSaleMonthRpt>(dt);
                                }
                            }
                        }
                    }
                    Common.InsertLog(Request[UserMobile]);
                    #endregion
                }
            }
            Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rd));
            Response.End();
            return;
        }
    }
}