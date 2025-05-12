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
    public class StoreRepo : IStoreRepo
    {
        private readonly EcommerceDBContext _dbContext;

        public StoreRepo(EcommerceDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CreateStore(Store newStore)
        {
            try
            {
                await _dbContext.Store.AddAsync(newStore);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while creating a store.", ex);
            }
        }

        public async Task<bool> DeactivateStore(int storeId, int userId)
        {
            try
            {
                var store = await _dbContext.Store
                    .FirstOrDefaultAsync(s => s.StoreID == storeId && s.UserID == userId);

                if (store == null) return false;

                store.IsActive = false;
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while deactivating store.", ex);
            }
        }

        public async Task<bool> StoreExistsByName(string storeName, int userId)
        {
            return await _dbContext.Store
                .AnyAsync(s => s.StoreName == storeName && s.UserID == userId && s.IsActive);
        }

        public async Task<bool> DeactivateStoreByName(string storeName, int userId)
        {
            try
            {
                var store = await _dbContext.Store
                    .Include(s => s.Addresses) 
                    .FirstOrDefaultAsync(s => s.StoreName == storeName && s.UserID == userId);

                if (store == null)
                    return false;

                store.IsActive = false;

                foreach (var address in store.Addresses)
                {
                    address.IsActive = false; 
                }

                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while deactivating store by name.", ex);
            }
        }


        public async Task<Store?> GetStoreByNameAndUser(string storeName, int userId)
        {
            return await _dbContext.Store
                .Include(s => s.Addresses)
                .FirstOrDefaultAsync(s => s.StoreName == storeName && s.UserID == userId && s.IsActive);
        }

        public async Task<bool> UpdateStore(Store store)
        {
            _dbContext.Store.Update(store);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UnassignBrandFromStore(string storeName, int userId)
        {
            var store = await _dbContext.Store
                .FirstOrDefaultAsync(s => s.StoreName == storeName && s.UserID == userId);

            if (store == null)
                return false;

            store.BrandId = null;

            return await _dbContext.SaveChangesAsync() > 0;
        }


    }
}
