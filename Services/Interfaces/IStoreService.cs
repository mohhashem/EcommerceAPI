using Domain.DTOs.ProductDTOs;
using Domain.DTOs.StoreDTOs;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IStoreService
    {
        Task<bool> CreateStore(NewStoreDto dto, int userId);
        Task<bool> DeleteStoreByName(string storeName, int userId);
        Task<bool> UpdateStore(UpdateStoreDto dto, int userId);
        Task<bool> UnassignBrand(string storeName, int userId);

    }
}
