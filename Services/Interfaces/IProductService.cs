using Domain.DTOs.ProductDTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IProductService
    {
        Task<bool> AddProduct(NewProductDTO dto, int userId);
        Task<bool> SoftDeleteProductByName(ProductIdentifierDTO dto, int userId);
        Task<bool> RecoverProductByName(ProductIdentifierDTO dto, int userId);
        Task<bool> EditProduct(EditProductDTO dto, int userId);
        Task<List<CachedProductDTO>> GetAllProductsByStoreAndBrand(string storeName, string brandName, int userId);


    }
}
