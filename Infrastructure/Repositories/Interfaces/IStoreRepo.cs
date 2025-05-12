using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IStoreRepo
    {
        Task<bool> CreateStore(Store store);
        Task<bool> DeactivateStore(int storeId, int userId);
        Task<Store?> GetStoreByNameAndUser(string storeName, int userId);
        Task<bool> StoreExistsByName(string storeName, int userId);
        Task<bool> UpdateStore(Store store);

        Task<bool> DeactivateStoreByName(string storeName, int userId);
        Task<bool> UnassignBrandFromStore(string storeName, int userId);

    }
}
