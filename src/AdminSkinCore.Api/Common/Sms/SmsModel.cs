using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Common.Sms
{
    /// <summary>
    /// 短信模型
    /// </summary>
    public class SmsModel
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNumbers { set; get; }
        /// <summary>
        /// 签名
        /// </summary>
        public string SignName { get; set; }
        /// <summary>
        /// 短信模板id
        /// </summary>
        public string TempletCode { set; get; }
        /// <summary>
        /// 短信模板中填充的数据
        /// </summary>
        public IDictionary<string, string> TemplateParam { set; get; }
        /// <summary>
        /// 上行短信扩展码
        /// </summary>
        public string SmsUpExtendCode { get; set; } = "";
        /// <summary>
        /// 流水ID
        /// </summary>
        public string OutId { set; get; } = "";
        /// <summary>
        /// AK信息
        /// </summary>
        public string AccessKeyId { get; set; }
        /// <summary>
        /// AK信息
        /// </summary>
        public string AccessKeySecret { get; set; }
    }
}
