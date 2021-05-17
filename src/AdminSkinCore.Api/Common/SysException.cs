using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Common
{
    /// <summary>
    /// 系统异常
    /// </summary>
    public class SysException : Exception
    {
        public SysException(string msg) : base(msg)
        {

        }
    }
}
