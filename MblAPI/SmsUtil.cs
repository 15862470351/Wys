using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Ultra.Web.Core.Common;

namespace MblAPI
{
	public class SmsUtil
	{
		private static string DbgVerfiy = "KY_SMS.txt";

		private static List<string> DbgMobileNum()
		{
			var fi = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbgVerfiy);
			if (!File.Exists(fi)) return null;

			var str = File.ReadAllText(fi, Encoding.UTF8);
			var s = str.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if (null == s || s.Length < 1) return null;
			return s.ToList();
		}

		public string SmsAccount
		{
			get;
			set;
		}
		public string SmsPassword
		{
			get;
			set;
		}
		public string SmsPassageName
		{
			get;
			set;
		}
		string _SmsWebServicesURL = string.Empty;
		public string SmsWebServicesURL
		{
			get
			{
				return string.IsNullOrEmpty(_SmsWebServicesURL) ? string.Format("{0}sendXSms.do", SmsWebSvrRootURL)
					: _SmsWebServicesURL;
			}
			set
			{
				_SmsWebServicesURL = value;
			}
		}

		public string SmsWebSvrRootURL { get { return "http://www.ztsms.cn:8800/"; } }

		public decimal QuerySmsAmount(out bool bErr, out string errMsg)
		{
			decimal result;
			try
			{
				bErr = false;
				string newProductId = this.GetNewProductId();
				string requestUriString = string.Empty;
				requestUriString = string.Format(SmsWebSvrRootURL + "balance.do?username={0}&password={1}&productid={2}", this.SmsAccount, this.SmsPassword, newProductId);
				//if (this.BackUpPId != newProductId)
				//{
				//    requestUriString = string.Format("http://www.ztsms.cn:8800/balance.do?username={0}&password={1}&productid={2}", this.SmsAccount, this.SmsPassword, newProductId);
				//}
				//else
				//{
				//    requestUriString = string.Format("http://117.135.160.56/balance.do?username={0}&password={1}&productid={2}", this.SmsAccount, this.SmsPassword, newProductId);
				//}
				HttpWebRequest httpWebRequest = WebRequest.Create(requestUriString) as HttpWebRequest;
				HttpWebResponse res = httpWebRequest.GetResponse() as HttpWebResponse;
				string responseString = this.GetResponseString(res);
				decimal num;
				if (decimal.TryParse(responseString, out num))
				{
					if (num < 0m)
					{
						errMsg = "用户名或密码不正确";
					}
					else
					{
						errMsg = string.Empty;
					}
					bErr = (num < 0m);
					result = num;
				}
				else
				{
					bErr = true;
					errMsg = responseString;
					result = 0m;
				}
			}
			catch (Exception ex)
			{
				bErr = true;
				errMsg = ex.ToString();
				result = -1m;
			}
			return result;
		}

		public decimal QuerySmsAmount()
		{
			bool flag;
			string text;
			return this.QuerySmsAmount(out flag, out text);
		}

		protected virtual string GetResponseString(HttpWebResponse res)
		{
			Stream responseStream = res.GetResponseStream();
			StreamReader streamReader;
			if (res.CharacterSet != null)
			{
				streamReader = new StreamReader(responseStream, Encoding.GetEncoding(res.CharacterSet));
			}
			else
			{
				streamReader = new StreamReader(responseStream);
			}
			return streamReader.ReadToEnd();
		}
		protected virtual string GetNewProductId()
		{
			//string result = "623147";
			//string smsPassageName;
			//if ((smsPassageName = this.SmsPassageName) != null)
			//{
			//    if (!(smsPassageName == "887361") && !(smsPassageName == "156041"))
			//    {
			//        if (!(smsPassageName == "251503") && !(smsPassageName == "178576"))
			//        {
			//            if (smsPassageName == "411623")
			//            {
			//                result = "411623";
			//            }
			//        }
			//        else
			//        {
			//            result = "251503";
			//        }
			//    }
			//    else
			//    {
			//        result = "887361";
			//    }
			//}
			//return result;
			return this.SmsPassageName;
		}

		protected virtual void SubmitSmsRequest(List<string> mobiles, string content, DateTime? dt, out string errmsg)
		{
			try
			{
				errmsg = string.Empty;
				if (null == mobiles || mobiles.Count < 1) return;
				string newProductId = this.GetNewProductId();
				errmsg = string.Empty;
				if (mobiles == null)
				{
					throw new ArgumentNullException("mobiles");
				}
				if (string.IsNullOrEmpty(content))
				{
					throw new ArgumentNullException("content");
				}
				string arg_A2_0 = "?username={0}&password={1}&mobile={2}&content={3}&productid={4}";
				object[] array = new object[5];
				array[0] = this.SmsAccount;
				array[1] = this.SmsPassword;
				array[2] = mobiles.Aggregate((string s1, string s2) => s1 + "," + s2).ToString();
				array[3] = content.GetUrlEncode();
				array[4] = (string.IsNullOrEmpty(this.SmsPassageName) ? "623147" : newProductId);
				string text = string.Format(arg_A2_0, array);
				if (dt.HasValue)
				{
					text += string.Format("&dstime={0}", dt.Value.ToDefaultStr());
				}
				HttpWebRequest httpWebRequest;
				//if (this.BackUpPId == newProductId)
				//{
				//    httpWebRequest = (WebRequest.Create(this.BackUpSmsWebServiceURL + text) as HttpWebRequest);
				//}
				//else
				{
					httpWebRequest = (WebRequest.Create(this.SmsWebServicesURL + text) as HttpWebRequest);
				}
				HttpWebResponse res = httpWebRequest.GetResponse() as HttpWebResponse;
				string responseString = this.GetResponseString(res);
				errmsg = this.TranslateMessage(responseString);
				//if (!string.IsNullOrEmpty(errmsg))
				//{
				//	string path = Path.Combine(Path.GetDirectoryName(Application.StartupPath), "SmsFail.txt");
				//	try
				//	{
				//		foreach (string current in mobiles)
				//		{
				//			File.AppendAllText(path, string.Concat(new string[]
				//			{
				//				DateTime.Now.ToDefaultStr(),
				//				" ",
				//				current,
				//				" ",
				//				errmsg,
				//				"\r\n"
				//			}));
				//		}
				//	}
				//	catch
				//	{
				//	}
				//}
			}
			catch (Exception ex)
			{
				errmsg = ex.ToString();
			}
		}

		public virtual bool SendSms(List<string> mobiles, string content, out string errmsg)
		{
			//调试处理
			var dbgnum = DbgMobileNum();
			if (null != dbgnum)//测试发送，变换手机号
			{
				errmsg = string.Empty;
				this.SubmitSmsRequest(dbgnum, content, null, out errmsg);
				return string.IsNullOrEmpty(errmsg);
			}
			else
			{
				errmsg = string.Empty;
				this.SubmitSmsRequest(mobiles, content, null, out errmsg);
				return string.IsNullOrEmpty(errmsg);
			}
		}

		public virtual bool SendSms(string mobile, string content, out string errmsg)
		{
			return this.SendSms(new List<string>
            {
                mobile
            }, content, out errmsg);
		}
		public virtual bool SendSms(string mobile, string content, DateTime dt, out string errmsg)
		{
			return this.SendSms(new List<string>
            {
                mobile
            }, content, dt, out errmsg);
		}
		public virtual bool SendSms(List<string> mobiles, string content, DateTime dt, out string errmsg)
		{
			//调试处理
			var dbgnum = DbgMobileNum();
			if (null != dbgnum)//测试发送，变换手机号
			{
				errmsg = string.Empty;
				this.SubmitSmsRequest(dbgnum, content, new DateTime?(dt), out errmsg);
				return string.IsNullOrEmpty(errmsg);
			}
			else
			{
				errmsg = string.Empty;
				this.SubmitSmsRequest(mobiles, content, new DateTime?(dt), out errmsg);
				return string.IsNullOrEmpty(errmsg);
			}
		}


		public virtual string TranslateMessage(string errSmsMsg)
		{
			errSmsMsg = errSmsMsg.Replace("\n", string.Empty);
			string[] array = errSmsMsg.Split(",".ToCharArray());
			if (array != null && array.Length > 0 && array[0] == "1")
			{
				return string.Empty;
			}
			string key;
			switch (key = array[0])
			{
				case "0":
					return "发送短信失败";
				case "-1":
					return "用户名或者密码不正确";
				case "2":
					return "余额不够";
				case "3":
					return "扣费失败（请联系客服）";
				case "6":
					return "有效号码为空";
				case "7":
					return "短信内容为空";
				case "8":
					return "无签名，必须，格式：【签名】";
				case "9":
					return "没有url提交权限";
				case "10":
					return "发送号码过多";
				case "11":
					return "产品ID异常";
				case "12":
					return "参数异常";
				case "13":
					return "30分种重复提交";
				//14	用户名或密码不正确，产品余额为0，禁止提交，联系客服
				//15	Ip验证失败
				//19	短信内容过长，最多支持500个
				//20	定时时间不正确：格式：20130202120212(14位数字)
				case "14":
					return "余额为0";
				case "15":
					return "Ip验证失败";
				case "19":
					return "短信内容过长";
				case "20":
					return "定时时间不正确：格式：20130202120212(14位数字)";

			}
			return string.Empty;
		}

		public virtual string TranslateBalanceMessage(string smsmsg, out decimal? dAmount)
		{
			if (smsmsg.StartsWith("-1"))
			{
				throw new Exception("账号或密码不正确");
			}
			dAmount = null;
			dAmount = new decimal?(0m);
			smsmsg = smsmsg.Replace("\n", string.Empty);
			string[] array = smsmsg.Split(";".ToCharArray());
			if (array != null && array.Length > 0)
			{
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text = array2[i];
					string[] array3 = text.Split(",".ToCharArray());
					if (array3 != null && array3.Length == 3)
					{
						if (array3[0] == this.SmsPassageName)
						{
							dAmount = array3[2].ToDecimal();
							break;
						}
						decimal? num = array3[2].ToDecimal();
						if (0m != num && num > 0m)
						{
							dAmount = num;
							return string.Empty;
						}
					}
				}
			}
			return this.TranslateMessage(smsmsg);
		}
	}
}