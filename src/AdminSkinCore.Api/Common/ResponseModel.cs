using AdminSkinCore.Api.Extension;
using AdminSkinCore.Api.Utility.CustomAttribute;

namespace AdminSkinCore.Api.Common
{
    public enum PublicStatusCode
    {
        [StatusCode(200, "成功")]
        Success,
        [StatusCode(400, "失败")]
        Fail,
        [StatusCode(500, "系统错误")]
        SystemError,
    }
    public class ResponseModel
    {
        /// <summary>
        /// 状态码
        /// 200成功，400错误，500系统错误
        /// 10001开始为每个接口的自定义状态码
        /// 默认500
        /// </summary>
        public int Code { get; set; } = 500;
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; } = "";
        public static ResponseModel BuildResponse(PublicStatusCode en)
        {
            var resp = new ResponseModel();
            var attr = en.GetAttribute<StatusCodeAttribute>();
            resp.Code = attr.Code;
            resp.Msg = attr.Desc;
            return resp;
        }
        public void UpdateCodeAndMsg(PublicStatusCode en)
        {
            var attr = en.GetAttribute<StatusCodeAttribute>();
            Code = attr.Code;
            Msg = attr.Desc;
        }
    }
    /// <summary>
    /// 带数据返回
    /// </summary>
    public class ResponseModel<T> : ResponseModel
    {
        /// <summary>
        /// 返回的数据
        /// </summary>
        public T Data { get; set; }
    }
}
