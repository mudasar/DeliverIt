using DeliverIt.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliverIt.Services
{
    public interface IPartnerService
    {
        Task<Partner> CreatePartner(Partner model);
        Task<IList<Partner>> GetAllPartners();
        Task<Partner> GetPartnerById(int id);
        Task<Partner> RemovePartner(int id);
        Task<Partner> UpdatePartner(Partner Partner);
        Task<bool> PartnerExists(int id);
        Task<bool> PartnerExists(string email);
    }
}
