using Domain.Entities;
using System.Linq.Expressions;


namespace Infrastructure.Repositories.Interfaces
{
    public interface IProductRepo
    {
        Task<bool> AddProduct(Product product);
        Task<Product?> GetProductByNameAndStore(string productName, string storeName, string brandName, int userId);
        Task<bool> UpdateProduct(Product product);
        Task<List<Product>> GetAllProductsByStoreAndBrand(string storeName, string brandName, int userId);

        Task<bool> UpdateProductFields(Product product, params Expression<Func<Product, object>>[] propertiesToUpdate);

    }
}
