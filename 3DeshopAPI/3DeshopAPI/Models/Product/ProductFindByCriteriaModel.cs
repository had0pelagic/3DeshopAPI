namespace _3DeshopAPI.Models.Product
{
    public class ProductFindByCriteriaModel
    {
        public string Name { get; set; }
        public List<string> Categories { get; set; }
        public ProductSpecificationsModel Specifications { get; set; }
    }
}
