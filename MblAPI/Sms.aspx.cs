using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ultra.Web.Core.Common;

namespace MblAPI
{
	public partial class Sms : System.Web.UI.Page
	{
		/// <summary>
		/// 参数：m:手机号,
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			//Response.Write(Common.GetVerfyCode());
			//Response.End();
			var mobile = Request["m"];
			if (string.IsNullOrEmpty(mobile))//没有传入手机号
			{
				Response.Write("No Mobile Num"); Response.End(); return;
			}
			var dt = SqlHelper.ExecuteDataTable(Common.DefConn, System.Data.CommandType.Text, "select top 1 * from t_erp_mobileaccount where isusing=1 and mobile=@0",
				new SqlParameter("@0", mobile));
			if (null == dt || dt.Rows.Count < 1) { Response.Write("No Mobile Found!"); Response.End(); return; }
			var vcd = Common.GetVerfyCode(); string ermsg;
			var vcdstr = string.Format("您的登录验证码为:{0} 【客易软件】", vcd);
			if (!Common.SendSms(mobile, vcdstr, out ermsg))
			{
				Response.Write(ermsg); Response.End(); return;
			}
			SqlHelper.ExecuteNonQuery(Common.DefConn, System.Data.CommandType.Text, "Update t_erp_mobileaccount set VerfyCode=@0,VerfyTime=GetDate() where Id=@1"
				, new SqlParameter("@0", vcd), new SqlParameter("@1", dt.Rows[0]["Id"]));
			Response.End(); return;
		}
	}
}