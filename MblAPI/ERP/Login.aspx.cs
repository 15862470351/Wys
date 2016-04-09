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
    public partial class Login : System.Web.UI.Page
    {
        /// <summary>
        /// 参数:mobile :手机号, code:验证码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            var mobile = Request["mobile"];
            var code = Request["code"];
            ResultData rd = new ResultData { IsOK = false };
            if (string.IsNullOrEmpty(mobile) || string.IsNullOrEmpty(code))
            {
                rd.ErrMsg = "无效参数";
            }
            else
            {
                var dt = SqlHelper.ExecuteDataTable(Common.DefConn, CommandType.Text, "select top 1 * from T_ERP_MobileAccount where isusing=1 and mobile=@0 and VerfyCode=@1",
                    new SqlParameter("@0", mobile), new SqlParameter("@1", code));
                if (dt == null || dt.Rows.Count < 1)
                {
                    rd.ErrMsg = "手机号码或验证码有误";
                }
                else
                {   

                    rd.IsOK = true;
                    rd.Data = Common.EncryptStr(dt.Rows[0]["DbConn"].ToString());
                    SqlHelper.ExecuteNonQuery(Common.DefConn, System.Data.CommandType.Text, "Update T_ERP_MobileAccount set LoginTime=GetDate() where Id=@0"
    , new SqlParameter("@0", dt.Rows[0]["Id"]));
                }
            }
            Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(rd));
            Response.End(); return;
        }
    }
}