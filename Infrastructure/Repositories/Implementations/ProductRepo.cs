using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Implementations
{
    public class ProductRepo: IProductRepo
    {
        private readonly EcommerceDBContext _dbContext;

        public ProductRepo(EcommerceDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddProduct(Product product)
        {
            await _dbContext.Product.AddAsync(product);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<Product?> GetProductByNameAndStore(string productName, string storeName, string brandName, int userId)
        {
            return await _dbContext.Product
                .Include(p => p.Store)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p =>
                    p.ProductName == productName &&
                    p.Store.StoreName == storeName &&
                    p.Store.UserID == userId &&
                    p.Brand.BrandName == brandName);
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            _dbContext.Product.Update(product);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateProductFields(Product product, params Expression<Func<Product, object>>[] propertiesToUpdate)
        {
            var entry = _dbContext.Entry(product);
            foreach (var prop in propertiesToUpdate)
            {
                entry.Property(prop).IsModified = true;
            }

            return await _dbContext.SaveChangesAsync() > 0;
        }


        public async Task<List<Product>> GetAllProductsByStoreAndBrand(string storeName, string brandName, int userId)
        {
            return await _dbContext.Product
                .Include(p => p.Store)
                .Include(p => p.Brand)
                .Where(p =>
                    p.Store.StoreName == storeName &&
                    p.Brand.BrandName == brandName &&
                    p.Store.UserID == userId && p.IsDeleted == false)
                .ToListAsync();
        }


    }
}
