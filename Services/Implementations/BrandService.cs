using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepo _brandRepo;

        public BrandService(IBrandRepo brandRepo)
        {
            _brandRepo = brandRepo;
        }

        public async Task<Brand?> GetBrandByName(string brandName)
        {
            if (string.IsNullOrWhiteSpace(brandName))
                return null;

            return await _brandRepo.GetBrandByName(brandName);
        }

        public async Task<Brand> CreateBrand(string brandName)
        {
            if (string.IsNullOrWhiteSpace(brandName))
                throw new ArgumentException("Brand name is required.");

            var existing = await _brandRepo.GetBrandByName(brandName);
            if (existing != null)
                throw new InvalidOperationException("Brand with the same name already exists.");

            var newBrand = new Brand { BrandName = brandName };

            var success = await _brandRepo.CreateBrand(newBrand);
            if (!success)
                throw new Exception("Failed to create brand.");

            return newBrand;
        }

        public async Task<bool> UpdateBrandName(string currentName, string newName)
        {
            if (string.IsNullOrWhiteSpace(currentName) || string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Both current and new brand names are required.");

            if (string.Equals(currentName, newName, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("New brand name must be different from current name.");

            var existingBrandWithNewName = await _brandRepo.GetBrandByName(newName);
            if (existingBrandWithNewName != null)
                throw new InvalidOperationException("A brand with the new name already exists.");

            var currentBrand = await _brandRepo.GetBrandByName(currentName);
            if (currentBrand == null)
                return false;

            currentBrand.BrandName = newName;

            return await _brandRepo.UpdateBrandName(currentBrand);
        }


    }
}
