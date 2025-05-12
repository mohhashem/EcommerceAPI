using Domain.DTOs.ProductDTOs;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepo _productRepo;
        private readonly IStoreRepo _storeRepo;
        private readonly IBrandService _brandService;
        private readonly ICacheService _cacheService;

        public ProductService(IProductRepo productRepo, IStoreRepo storeRepo, IBrandService brandService, ICacheService cacheService)
        {
            _productRepo = productRepo;
            _storeRepo = storeRepo;
            _brandService = brandService;
            _cacheService = cacheService;
        }

        public async Task<bool> AddProduct(NewProductDTO dto, int userId)
        {
            var cacheKey = GetProductCacheKey(dto.StoreName, dto.BrandName, userId);
            var cachedList = await _cacheService.GetAsync<List<CachedProductDTO>>(cacheKey);

            var cachedProduct = cachedList?.FirstOrDefault(p =>
                p.ProductName.Equals(dto.ProductName, StringComparison.OrdinalIgnoreCase) &&
                p.StoreName.Equals(dto.StoreName, StringComparison.OrdinalIgnoreCase) &&
                p.BrandName.Equals(dto.BrandName, StringComparison.OrdinalIgnoreCase));

            if (cachedProduct is not null && !cachedProduct.IsDeleted)
                throw new InvalidOperationException("Product already exists in cache.");

            var existingProduct = await _productRepo.GetProductByNameAndStore(dto.ProductName, dto.StoreName, dto.BrandName, userId);

            if (existingProduct is not null)
            {
                if (!existingProduct.IsDeleted)
                    throw new InvalidOperationException("Product already exists in database.");

                var recovered = await RecoverProductByName(new ProductIdentifierDTO
                {
                    ProductName = dto.ProductName,
                    StoreName = dto.StoreName,
                    BrandName = dto.BrandName
                }, userId);

                return recovered;
            }

            var store = await _storeRepo.GetStoreByNameAndUser(dto.StoreName, userId);
            if (store == null)
                throw new ArgumentException("Store not found or you do not own this store.");

            var brand = await _brandService.GetBrandByName(dto.BrandName);
            if (brand == null)
                throw new ArgumentException($"Brand '{dto.BrandName}' not found.");

            var product = new Product
            {
                ProductName = dto.ProductName,
                ImageURL = dto.ImageURL,
                Price = dto.Price,
                StoreId = store.StoreID,
                BrandId = brand.BrandId,
                IsDeleted = false
            };

            var result = await _productRepo.AddProduct(product);
            if (!result)
                return false;

            await RefreshProductCache(dto.StoreName, dto.BrandName, userId);
            return true;
        }



        private async Task<bool> SetProductDeletedState(ProductIdentifierDTO dto, int userId, bool isDeleted)
        {
            var product = await _productRepo.GetProductByNameAndStore(dto.ProductName, dto.StoreName, dto.BrandName, userId);
            if (product == null)
                throw new ArgumentException("Product not found or access denied.");

            product.IsDeleted = isDeleted;
            var result = await _productRepo.UpdateProductFields(product, p => p.IsDeleted);

            if (result)
                await RefreshProductCache(dto.StoreName, dto.BrandName, userId);

            return result;
        }

        public Task<bool> SoftDeleteProductByName(ProductIdentifierDTO dto, int userId) =>
            SetProductDeletedState(dto, userId, true);

        public Task<bool> RecoverProductByName(ProductIdentifierDTO dto, int userId) =>
            SetProductDeletedState(dto, userId, false);



        public async Task<bool> EditProduct(EditProductDTO dto, int userId)
        {
            var product = await _productRepo.GetProductByNameAndStore(dto.ProductName, dto.StoreName, dto.BrandName, userId);

            if (product == null)
                throw new ArgumentException("Product not found or access denied.");

            product.ProductName = dto.NewProductName;
            product.Price = dto.NewPrice;
            product.ImageURL = dto.NewImageURL;

            var result = await _productRepo.UpdateProductFields(
                 product,
                 p => p.ProductName,
                 p => p.Price,
                 p => p.ImageURL);

            if (result)
                await RefreshProductCache(dto.StoreName, dto.BrandName, userId);

            return result;
        }

        public async Task<List<CachedProductDTO>> GetAllProductsByStoreAndBrand(string storeName, string brandName, int userId)
        {
            var cacheKey = GetProductCacheKey(storeName, brandName, userId);

            var cached = await _cacheService.GetAsync<List<CachedProductDTO>>(cacheKey);
            if (cached != null && cached.Any())
                return cached.Where(p => !p.IsDeleted).ToList(); 

            var products = await _productRepo.GetAllProductsByStoreAndBrand(storeName, brandName, userId);
            if (products == null || !products.Any())
                throw new InvalidOperationException("No products found.");

            var dtoList = products.Select(p => new CachedProductDTO
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                ImageURL = p.ImageURL,
                Price = p.Price,
                IsDeleted = p.IsDeleted,
                StoreName = p.Store.StoreName,
                BrandName = p.Brand.BrandName
            }).ToList();

            await _cacheService.SetAsync(cacheKey, dtoList, TimeSpan.FromMinutes(30));
            return dtoList.Where(p => !p.IsDeleted).ToList(); 
        }


        private async Task RefreshProductCache(string storeName, string brandName, int userId)
        {
            var products = await _productRepo.GetAllProductsByStoreAndBrand(storeName, brandName, userId);

            var dtoList = products.Select(p => new CachedProductDTO
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                ImageURL = p.ImageURL,
                Price = p.Price,
                IsDeleted = p.IsDeleted,
                StoreName = p.Store.StoreName,
                BrandName = p.Brand.BrandName
            }).ToList();

            var cacheKey = GetProductCacheKey(storeName, brandName, userId);
            await _cacheService.SetAsync(cacheKey, dtoList, TimeSpan.FromMinutes(30));
        }


        private string GetProductCacheKey(string storeName, string brandName, int userId)
        {
            return $"products:{userId}:{storeName.Trim().ToLower()}:{brandName.Trim().ToLower()}";
        }

    }

}
