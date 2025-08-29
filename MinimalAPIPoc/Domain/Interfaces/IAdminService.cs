using MinimalAPIPoc.Domain.DTO;
using MinimalAPIPoc.Domain.Entities;

namespace MinimalAPIPoc.Domain.Interfaces
{
    public interface IAdminService
    {
        Admin Login(LoginDTO loginDTO);

    }
}
