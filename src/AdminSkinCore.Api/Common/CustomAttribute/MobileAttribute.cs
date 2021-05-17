using System.ComponentModel.DataAnnotations;

namespace AdminSkinCore.Api.Utility.CustomAttribute
{
    public class MobileAttribute : RegularExpressionAttribute
    {
        public MobileAttribute() : base(@"^[1]+\d{10}")
        {
            this.ErrorMessage = "手机号格式错误";
        }
    }
}
