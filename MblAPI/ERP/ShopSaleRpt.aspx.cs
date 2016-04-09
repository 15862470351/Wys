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
    public partial class ShopSaleRpt : BasePage
	{
		//按店铺区分不同店铺的　日销售额，月销售额
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
                    if (num == "1") //日销售额
                    {
                        if (string.IsNullOrEmpty(time))
                        {
                            var dt = SqlHelper.ExecuteDataTable(cnn, CommandType.Text, "select * from V_ERP_RptSalesDayZP  where DATEPART(YEAR,[日期])=@0 and  DATEPART(DAY,[日期])=@1",
                       new SqlParameter("@0", DateTime.Now.Year), new SqlParameter("@1", DateTime.Now.Day));
                            if (dt == null || dt.Rows.Count < 1)
                            {
                                rd.ErrMsg = "无有效数据";
                            }
                            else
                            {
                                rd.IsOK = true;
                                rd.Data = Ultra.Web.Core.Common.ObjectHelper.Create<RptSales>(dt);
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
                                var dt = SqlHelper.ExecuteDataTable(cnn, CommandType.Text, "select * from V_ERP_RptSalesDayZP  where DATEPART(YEAR,[日期])=@0 and  DATEPART(DAY,[日期])=@1",
                       new SqlParameter("@0", ti.Year), new SqlParameter("@1", ti.Day));
                                if (dt == null || dt.Rows.Count < 1)
                                {
                                    rd.ErrMsg = "无有效数据";
                                }
                                else
                                {
                                    rd.IsOK = true;
                                    var data = Ultra.Web.Core.Common.ObjectHelper.Create<RptSales>(dt);
                                    rd.Data = data;
                                }
                            }
                        }
                    }
                    else if (num == "2")//月销售额
                    {
                        if (string.IsNullOrEmpty(time))
                        {
                            var dt = SqlHelper.ExecuteDataTable(cnn, CommandType.Text, "select * from V_ERP_RptSalesMonthZP  where DATEPART(YEAR,[日期])=@0 and  DATEPART(MONTH,[日期])=@1",
                       new SqlParameter("@0", DateTime.Now.Year), new SqlParameter("@1", DateTime.Now.Month));
                            if (dt == null || dt.Rows.Count < 1)
                            {
                                rd.ErrMsg = "无有效数据";
                            }
                            else
                            {
                                rd.IsOK = true;
                                rd.Data = Ultra.Web.Core.Common.ObjectHelper.Create<RptSales>(dt);
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
                                var dt = SqlHelper.ExecuteDataTable(cnn, CommandType.Text, "select * from V_ERP_RptSalesMonthZP  where DATEPART(YEAR,[日期])=@0 and  DATEPART(MONTH,[日期])=@1",
                       new SqlParameter("@0", ti.Year), new SqlParameter("@1", ti.Month));
                                if (dt == null || dt.Rows.Count < 1)
                                {
                                    rd.ErrMsg = "无有效数据";
                                }
                                else
                                {
                                    rd.IsOK = true;
                                    rd.Data = Ultra.Web.Core.Common.ObjectHelper.Create<RptSales>(dt);
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