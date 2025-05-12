using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ImageURL { get; set; }
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }

        public int StoreId { get; set; }
        public Store Store { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }
    }

}
