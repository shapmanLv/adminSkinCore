using AutoMapper;
using AdminSkinCore.Api.Dto;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using AdminSkinCore.Api.ViewModel;
using System.Collections.Generic;

namespace AdminSkinCore.Api.AutoMapper
{
    public class CustomProfile : Profile
    {
        /// <summary>
        /// 配置构造函数，用来创建关系映射
        /// </summary>
        public CustomProfile()
        {
            CreateMap<AddUserRequest, User>();
            CreateMap<AddAuthorizeApiRequest, AuthorizeApi>();
            CreateMap<EditAuthorizeApiRequest, AuthorizeApi>();
            CreateMap<AuthorizeApi, AuthorizeApiInfo>();
            CreateMap<Role, RoleInfo>();
            CreateMap<User, UserInfo>();
            CreateMap<Role, RoleBasicInfo>();
            CreateMap<User, UserBasicInfo>();
            CreateMap<EditUserRequest, User>();
            CreateMap<EditRoleRequest, Role>();
            CreateMap<AddRoleRequest, Role>();
            CreateMap<AddUserRequest, User>();
            CreateMap<AddMenuRequest, Menu>();
            CreateMap<Menu, MenuTree>();
            CreateMap<EditMenuRequest, Menu>();
        }
    }
}
