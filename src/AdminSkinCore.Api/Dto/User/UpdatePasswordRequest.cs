using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Dto
{
    public class UpdatePasswordRequest
    {
        /// <summary>
        /// 原始密码
        /// </summary>
        public string OriginalPassword { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; }
    }
}
