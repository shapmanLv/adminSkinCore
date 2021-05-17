using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Dto
{
    /// <summary>
    /// 菜单编辑 数据传输对象
    /// </summary>
    public class EditMenuRequest : AddMenuRequest
    {
        /// <summary>
        /// 菜单id
        /// </summary>
        public long Id { get; set; }
    }
}
