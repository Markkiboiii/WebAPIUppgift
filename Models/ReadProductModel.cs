namespace WebAPIUppgift.Models
{
    public class ReadProductModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        
    }
}
