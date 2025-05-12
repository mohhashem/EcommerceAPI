using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Address
    {
        public int AddressId { get; set; } 

        public string AddressLine { get; set; } 
        public string City { get; set; }
        public string State { get; set; }
        public int ZipCode { get; set; }

        public bool IsActive { get; set; } = true;

        public int StoreId { get; set; }
        public Store Store { get; set; }
    }
}
