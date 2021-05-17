using System;
using System.Reflection;

namespace AdminSkinCore.Api.Extension
{
    public static class EnumExtensions
    {
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attribute = (T)memberInfo[0].GetCustomAttribute(typeof(T), false);
            return attribute;
        }
    }
}
