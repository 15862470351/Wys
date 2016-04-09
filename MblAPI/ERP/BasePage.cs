using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Ultra.Web.Core.Common;

namespace MblAPI.ERP
{
    public class BasePage : System.Web.UI.Page
    {
        /// <summary>
        /// Token 参数名
        /// </summary>
        public string Token_ParamName = "Token";

        /// <summary>
        /// 用户手机号
        /// </summary>
        public string UserMobile = "mobile";
        /// <summary>
        /// check token data
        /// </summary>
        /// <param name="srcToken"></param>
        /// <returns></returns>
        public virtual bool CheckDbCnn(out string srcToken)
        {
            var cn = Request[Token_ParamName];
            srcToken = string.Empty;
            if (string.IsNullOrEmpty(cn)) return false;
            try
            {
                srcToken = Common.DecryptStr(cn);
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// 检查用户是否存在
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckMobile()
        {
            var mobile = Request[UserMobile];
            if (string.IsNullOrEmpty(mobile)) return false;
            var dt = SqlHelper.ExecuteDataTable(Common.DefConn, CommandType.Text, "select top 1 * from T_ERP_MobileAccount where isusing=1 and mobile=@0",
                 new SqlParameter("@0", mobile));
            if (dt == null || dt.Rows.Count < 1)
            {
                return false;
            }
            return true;
        }

        public bool CheckLogin()
        {
            var mobile = Request[UserMobile];
            if (string.IsNullOrEmpty(mobile)) return false;
            var dt = SqlHelper.ExecuteDataTable(Common.DefConn, CommandType.Text, "select top 1 * from T_ERP_MobileAccount where isusing=1 and mobile=@0",
                 new SqlParameter("@0", mobile));
            if (dt == null || dt.Rows.Count < 1)
            {
                return false;
            }
            else
            {
                DateTime time;
                if (DateTime.TryParse(dt.Rows[0]["LoginTime"].ToString(), out time))
                {
                    TimeSpan ts = DateTime.Now.Subtract(time).Duration();
                    if (ts.Hours >= 24)
                    {
                        return false;
                    }
                    else
                        return true;
                }
                return false;
            }
        }
    }
}