namespace Domain.DTOs.ProductDTOs
{
    public class EditProductDTO
    {
        public string StoreName { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }

        public string NewProductName { get; set; }
        public decimal NewPrice { get; set; }
        public string NewImageURL { get; set; }
    }
}
