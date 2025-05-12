namespace Domain.DTOs.ProductDTOs
{
    public class NewProductDTO
    {
        public string ProductName { get; set; }
        public string ImageURL { get; set; }
        public decimal Price { get; set; }

        public string StoreName { get; set; }
        public string BrandName { get; set; }
    }
}
