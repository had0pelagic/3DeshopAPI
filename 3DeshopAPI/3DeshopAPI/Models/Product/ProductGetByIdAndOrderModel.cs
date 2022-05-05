namespace _3DeshopAPI.Models.Product
{
    public class ProductGetByIdAndOrderModel
    {
        public List<string> ProductIds { get; set; }
        public bool Ascending { get; set; } = true;
    }
}
