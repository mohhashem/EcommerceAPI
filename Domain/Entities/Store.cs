using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Store
    {
        public int StoreID { get; set; }
        public string StoreName { get; set; }

        public string LogoUrl { get; set; }

        public int? BrandId { get; set; }
        public Brand Brand { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();

    }
}
