using DeliverIt.Data;
using DeliverIt.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliverIt.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly DeliverItContext db;

        public PartnerService(DeliverItContext context)
        {
            this.db = context;
        }

        public async Task<bool> PartnerExists(int id)
        {
            return await db.Partners.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> PartnerExists(string name)
        {
            // not best strategy to compare but quick one
            return await db.Partners.AnyAsync(x => x.Name.ToLower() == name.ToLower());
        }

        public async Task<Partner> AuthenticatePartner(string name, string password)
        {
            if (await this.PartnerExists(name))
            {
                // not best strategy to compare but quick one
                return await this.db.Partners.FirstOrDefaultAsync(x =>
                    x.Name.ToLower() == name.ToLower() &&
                    x.Password.ToLower() == password.ToLower());
            }
            else
            {
                return null;
            }
        }

        public async Task<IList<Partner>> GetAllPartners()
        {
            return await db.Partners.ToListAsync<Partner>();
        }

        public async Task<Partner> GetPartnerById(int id)
        {
            return await db.Partners.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Partner> CreatePartner(Partner partner)
        {
            db.Partners.Add(partner);
            await db.SaveChangesAsync();
            return partner;
        }

        public async Task<Partner> UpdatePartner(Partner partner)
        {
            db.Partners.Update(partner);
            await db.SaveChangesAsync();
            return partner;
        }

        public async Task<Partner> RemovePartner(int id)
        {
            var partner = await this.GetPartnerById(id);
            db.Partners.Remove(partner);
            await db.SaveChangesAsync();
            return partner;
        }
    }
}
