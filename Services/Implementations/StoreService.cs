using Domain.DTOs.StoreDTOs;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class StoreService : IStoreService
    {
        private readonly IStoreRepo _storeRepo;
        private readonly IBrandService _brandService;

        public StoreService(IStoreRepo storeRepo, IBrandService brandService)
        {
            _storeRepo = storeRepo;
            _brandService = brandService;
        }

        public async Task<bool> CreateStore(NewStoreDto dto, int userId)
        {
            var exists = await _storeRepo.StoreExistsByName(dto.StoreName, userId);
            if (exists)
                throw new InvalidOperationException("You already have a store with this name.");

            var brand = await _brandService.GetBrandByName(dto.BrandName);
            if (brand == null && !string.IsNullOrWhiteSpace(dto.BrandName))
                brand = await _brandService.CreateBrand(dto.BrandName);

            var store = new Store
            {
                StoreName = dto.StoreName,
                LogoUrl = dto.LogoUrl,
                IsActive = true,
                UserID = userId,
                BrandId = brand?.BrandId,
                Addresses = new List<Address>()
            };

            if (dto.Address != null)
            {
                store.Addresses.Add(new Address
                {
                    City = dto.Address.City,
                    AddressLine = dto.Address.AddressLine,
                    State = dto.Address.State,
                    ZipCode = dto.Address.ZipCode
                });
            }

            return await _storeRepo.CreateStore(store);
        }


        public async Task<bool> DeleteStoreByName(string storeName, int userId)
        {
            return await _storeRepo.DeactivateStoreByName(storeName, userId);
        }

        public async Task<bool> UpdateStore(UpdateStoreDto dto, int userId)
        {
            var store = await _storeRepo.GetStoreByNameAndUser(dto.CurrentStoreName, userId);
            if (store == null)
                return false;


            var exists = await _storeRepo.StoreExistsByName(dto.NewStoreName, userId);
            if (exists)
                throw new InvalidOperationException("You already have a store with this new name.");


            var brand = await _brandService.GetBrandByName(dto.BrandName);
            store.StoreName = dto.NewStoreName;
            store.LogoUrl = dto.LogoUrl;
            store.BrandId = brand?.BrandId;

            if (dto.Address != null)
            {
                var dtoAddr = dto.Address;



                var existingAddress = store.Addresses.FirstOrDefault(a => a.StoreId == store.StoreID);

                if (existingAddress == null)
                {
                    throw new InvalidOperationException($"Address associated store {store.StoreName} was not found in this store.");
                }

                existingAddress.AddressLine = dtoAddr.AddressLine;
                existingAddress.City = dtoAddr.City;
                existingAddress.State = dtoAddr.State;
                existingAddress.ZipCode = dtoAddr.ZipCode;


            }

            return await _storeRepo.UpdateStore(store);
        }




        public async Task<bool> UnassignBrand(string storeName, int userId)
        {
            return await _storeRepo.UnassignBrandFromStore(storeName, userId);
        }


    }
}
