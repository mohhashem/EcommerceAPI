namespace Domain.DTOs.StoreDTOs
{
    public class UpdateAddressDto
    {
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int ZipCode { get; set; }
    }

    public class UpdateStoreDto
    {
        public string CurrentStoreName { get; set; }
        public string NewStoreName { get; set; }
        public string LogoUrl { get; set; }
        public string BrandName { get; set; }

        public UpdateAddressDto Address { get; set; }  
    }
}
