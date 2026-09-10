namespace CoffeeNChillFunctions.Models
{
   
    public class MenuItemCreateRequest
    {
        public string Category { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public bool IsAvailable { get; set; }
    }

  
    public class MenuItemUpdateRequest
    {
        public double? Price { get; set; }
        public bool? IsAvailable { get; set; }
    }
}