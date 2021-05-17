using System;

namespace AdminSkinCore.Api.Utility.CustomAttribute
{
    public class StatusCodeAttribute : Attribute
    {
        public StatusCodeAttribute(int code, string desc = "")
        {
            Code = code;
            Desc = desc;
        }
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }
    }
}
