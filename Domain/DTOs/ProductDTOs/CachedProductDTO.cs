using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.ProductDTOs
{
    public class CachedProductDTO
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }

        public string StoreName{ get; set; }
        public string BrandName { get; set; }
    }
}
