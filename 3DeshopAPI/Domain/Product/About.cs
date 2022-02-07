namespace Domain.Product
{
    public class About
    {
        public Guid Id { get; set; }
        public string Name { get; set; }//product full name, visible on web site
        public DateTime UploadDate { get; set; }//product full name, visible on web site
        public double Price { get; set; }
        public string Description { get; set; }//product desc
        public int? Downloads { get; set; }
        //public ProductCategories Categories { get; set; }//categories given by user
        //public List<string> ImageUrls { get; set; }//images given by user
        //public List<Comment>? Comments { get; set; }
    }
}
