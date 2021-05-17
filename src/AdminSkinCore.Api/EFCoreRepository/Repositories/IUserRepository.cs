using AdminSkinCore.Api.EFCoreRepository.Base;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.EFCoreRepository.Repositories
{
    public interface IUserRepository : IRepository<User, long>
    {
    }
}
