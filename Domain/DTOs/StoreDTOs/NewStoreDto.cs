namespace Domain.DTOs.StoreDTOs
{
    public class AddressDto
    {
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int ZipCode { get; set; }
    }

    public class NewStoreDto
    {
        public string StoreName { get; set; }
        public string LogoUrl { get; set; }
        public string BrandName { get; set; }

        public AddressDto Address { get; set; }
    }
}
