using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Ultra.Web.Core.Common;

namespace MblAPI
{
	public static class Common
	{
		public static string GetUrlEncode(this string url)
		{
			return HttpUtility.UrlEncode(url);
		}
        static Ultra.Web.Core.Common.DESEncrypt rec = new Ultra.Web.Core.Common.DESEncrypt();

        public static string EncryptStr(string str)
        {
            return rec.EncryptString(str);
        }

        public static string DecryptStr(string str)
        {
            return rec.DecryptString(str);
        }

		public static string GetUrlDeCode(this string url)
		{
			return HttpUtility.UrlDecode(url);
		}

		public static string BytStr(byte[] bts)
		{
			string st = string.Empty;
			foreach (var bt in bts)
			{
				st += String.Format("{0:x2}", bt);  
			}
			return st;
		}

		public static readonly string DefConn = "User ID=sa;Initial Catalog=UltraERP;Data Source=101.251.96.119,4333;pwd=sa!@#123";
		public static readonly string SmsAccount = "KYsoft";
		public static readonly string SmsPwd = BytStr(Ultra.Web.Core.Common.HashDigest.StringDigest("F0Wp8TLn"));
		public static readonly string ProductId = "676767";
		public static List<string> VCode = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
		public static int Random(this int maxValue, System.Security.Cryptography.RNGCryptoServiceProvider rng = null)
		{
			rng = rng ?? new System.Security.Cryptography.RNGCryptoServiceProvider();
			decimal _base = (decimal)long.MaxValue;
			byte[] rndSeries = new byte[8];
			rng.GetBytes(rndSeries);
			return (int)(Math.Abs(BitConverter.ToInt64(rndSeries, 0)) / _base * maxValue);
		}

		public static bool SendSms(string mobile, string content, out string err)
		{
			SmsUtil sms = new SmsUtil();
			sms.SmsAccount = SmsAccount; sms.SmsPassword = SmsPwd; sms.SmsPassageName = ProductId;

			return sms.SendSms(mobile, content, out err);
		}

		/// <summary>
		/// 生成验证码
		/// </summary>
		/// <returns></returns>
		public static string GetVerfyCode()
		{
			var it = VCode.Count - 1;
			string sb = string.Empty;
			for (int i = 0; i <= 5; i++)
			{
				sb += VCode[Random(it)];
			}
			return sb;
		}
        /// <summary>
        /// 插入调用记录
        /// </summary>
        public static void InsertLog(string mobile)
        {
            if (!string.IsNullOrEmpty(mobile))
            {
                string url = HttpContext.Current.Request.Path;
                var result = SqlHelper.ExecuteNonQuery(Common.DefConn, CommandType.Text, "insert into T_ERP_InterfaceLog(mobile,InterfaceName,InvokeTime)values(@0,@1,@2)",
                    new SqlParameter("@0", mobile),
                     new SqlParameter("@1", url),
                      new SqlParameter("@2", DateTime.Now));
            }
        }
	}
}