using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AdminSkinCore.Api.Common.Sms
{
    /// <summary>
    /// 阿里云短信服务商 短信发送
    /// 代码来源： https://www.cnblogs.com/kulend/p/8807716.html
    /// </summary>
    public class AliyunSmsSender : ISmsSender
    {
        private string _regionId = "cn-hangzhou"; // 这个表示的是，服务商那边对于这条短信的发送，使用的服务器是 华东1（杭州）
        private string _version = "2017-05-25"; // 短信服务商api版本
        private string _action = "SendSms"; // 短信服务商单条短信发送api的action
        private string _format = "JSON"; // 数据格式化方式
        private string _domain = "dysmsapi.aliyuncs.com"; // 请求的时候的域名信息
        
        private bool _autoRetry = true; // 请求失败的时候是否允许重发
        private int _maxRetryNumber = 3; // 允许的重发次数
        private const string _separator = "&";  // url中querystring分隔符
        private int _timeoutInMilliSeconds = 100000; // 请求的超时时间限定

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="sms"></param>
        /// <returns></returns>
        public async Task<(bool success, string response)> Send(SmsModel sms)
        {
            var paramers = new Dictionary<string, string>();
            paramers.Add("PhoneNumbers", sms.PhoneNumbers);
            paramers.Add("SignName", sms.SignName);
            paramers.Add("TemplateCode", sms.TempletCode);
            paramers.Add("TemplateParam", JsonConvert.SerializeObject(sms.TemplateParam));
            paramers.Add("SmsUpExtendCode", sms.SmsUpExtendCode);
            paramers.Add("OutId", sms.OutId);
            paramers.Add("AccessKeyId", sms.AccessKeyId);

            try
            {
                string url = GetSignUrl(paramers, sms.AccessKeySecret);

                int retryTimes = 1;
                var reply = await HttpGetAsync(url);
                while (500 <= reply.StatusCode && _autoRetry && retryTimes < _maxRetryNumber)
                {
                    url = GetSignUrl(paramers, sms.AccessKeySecret);
                    reply = await HttpGetAsync(url);
                    retryTimes++;
                }

                if (!string.IsNullOrEmpty(reply.response))
                {
                    var res = JsonConvert.DeserializeObject<Dictionary<string, string>>(reply.response);
                    if (res != null && res.ContainsKey("Code") && "OK".Equals(res["Code"]))
                    {
                        return (true, response: reply.response);
                    }
                }

                return (false, response: reply.response);
            }
            catch (Exception ex)
            {
                return (false, response: ex.Message);
            }
        }

        /// <summary>
        /// 对请求进行签名
        /// </summary>
        /// <param name="parameters">url中的querystring</param>
        /// <param name="accessSecret">加密时的secret</param>
        /// <returns></returns>
        private string GetSignUrl(Dictionary<string, string> parameters, string accessSecret)
        {
            var imutableMap = new Dictionary<string, string>(parameters);
            imutableMap.Add("Timestamp", FormatIso8601Date(DateTime.Now));
            imutableMap.Add("SignatureMethod", "HMAC-SHA1");
            imutableMap.Add("SignatureVersion", "1.0");
            imutableMap.Add("SignatureNonce", Guid.NewGuid().ToString());
            imutableMap.Add("Action", _action);
            imutableMap.Add("Version", _version);
            imutableMap.Add("Format", _format);
            imutableMap.Add("RegionId", _regionId);

            IDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>(imutableMap, StringComparer.Ordinal);
            StringBuilder canonicalizedQueryString = new StringBuilder();
            foreach (var p in sortedDictionary)
            {
                canonicalizedQueryString.Append("&")
                .Append(PercentEncode(p.Key)).Append("=")
                .Append(PercentEncode(p.Value));
            }

            StringBuilder stringToSign = new StringBuilder();
            stringToSign.Append("GET");
            stringToSign.Append(_separator);
            stringToSign.Append(PercentEncode("/"));
            stringToSign.Append(_separator);
            stringToSign.Append(PercentEncode(canonicalizedQueryString.ToString().Substring(1)));

            string signature = SignString(stringToSign.ToString(), accessSecret + "&");

            imutableMap.Add("Signature", signature);

            return ComposeUrl(_domain, imutableMap);
        }

        private static string FormatIso8601Date(DateTime date)
        {
            return date.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.CreateSpecificCulture("en-US"));
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="source">需要加密的数据</param>
        /// <param name="accessSecret">secret</param>
        /// <returns></returns>
        public static string SignString(string source, string accessSecret)
        {
            using (var algorithm = new HMACSHA1(Encoding.UTF8.GetBytes(accessSecret.ToCharArray())))
            {
                return Convert.ToBase64String(algorithm.ComputeHash(Encoding.UTF8.GetBytes(source.ToCharArray())));
            }
        }

        /// <summary>
        /// 拼接请求的URL
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static string ComposeUrl(string endpoint, Dictionary<String, String> parameters)
        {
            StringBuilder urlBuilder = new StringBuilder("");
            urlBuilder.Append("http://").Append(endpoint);
            if (-1 == urlBuilder.ToString().IndexOf("?"))
            {
                urlBuilder.Append("/?");
            }
            string query = ConcatQueryString(parameters);
            return urlBuilder.Append(query).ToString();
        }

        /// <summary>
        /// 拼接 url 中的querystring
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static string ConcatQueryString(Dictionary<string, string> parameters)
        {
            if (null == parameters)
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();

            foreach (var entry in parameters)
            {
                String key = entry.Key;
                String val = entry.Value;

                sb.Append(HttpUtility.UrlEncode(key, Encoding.UTF8));
                if (val != null)
                {
                    sb.Append("=").Append(HttpUtility.UrlEncode(val, Encoding.UTF8));
                }
                sb.Append("&");
            }

            int strIndex = sb.Length;
            if (parameters.Count > 0)
                sb.Remove(strIndex - 1, 1);

            return sb.ToString();
        }

        public static string PercentEncode(string value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(value);
            foreach (char c in bytes)
            {
                if (text.IndexOf(c) >= 0)
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append("%").Append(
                        string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)c));
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 发送异步get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<(int StatusCode, string response)> HttpGetAsync(string url)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.Proxy = null;
            handler.AutomaticDecompression = DecompressionMethods.GZip;

            using (var http = new HttpClient(handler))
            {
                http.Timeout = new TimeSpan(TimeSpan.TicksPerMillisecond * _timeoutInMilliSeconds);
                HttpResponseMessage response = await http.GetAsync(url);
                return ((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }
    }
}
