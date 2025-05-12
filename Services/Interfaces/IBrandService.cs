using Domain.Entities;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBrandService
    {
        Task<Brand?> GetBrandByName(string brandName);
        Task<Brand> CreateBrand(string brandName);
        Task<bool> UpdateBrandName(string currentName, string newName);

    }
}
