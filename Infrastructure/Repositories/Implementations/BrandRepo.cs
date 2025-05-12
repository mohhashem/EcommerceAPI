using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations
{
    public class BrandRepo : IBrandRepo
    {
        private readonly EcommerceDBContext _context;
        public BrandRepo(EcommerceDBContext dBContext)
        {
            _context = dBContext;
        }
        public async Task<bool> CreateBrand(Brand brand)
        {
            try
            {
                await _context.Brand.AddAsync(brand);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while creating a brand.", ex);
            }
        }

        public async Task<Brand?> GetBrandByName(string name)
        {
            try
            {
                return await _context.Brand.FirstOrDefaultAsync(b => b.BrandName == name);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while retrieving the brand by name.", ex);
            }
        }

        public async Task<bool> UpdateBrandName(Brand brand)
        {
            _context.Brand.Update(brand);
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
