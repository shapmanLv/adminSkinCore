using System.ComponentModel.DataAnnotations;

namespace AdminSkinCore.Api.Utility.CustomAttribute
{
    public class EmailAttribute : RegularExpressionAttribute
    {
        public EmailAttribute() : base(@"^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$")
        {
            this.ErrorMessage = "邮箱格式错误";
        }
    }
}
