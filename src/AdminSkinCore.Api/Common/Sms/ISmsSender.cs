using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Common.Sms
{
    public interface ISmsSender
    {
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="sms">短信对象</param>
        /// <returns></returns>
        Task<(bool success, string response)> Send(SmsModel sms);
    }
}
