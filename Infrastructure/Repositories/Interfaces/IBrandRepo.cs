using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IBrandRepo
    {
        Task<Brand?> GetBrandByName(string brandName);
        Task<bool> CreateBrand(Brand brand);
        Task<bool> UpdateBrandName(Brand brand);

    }
}
