namespace Domain.Product
{
    public class Specifications
    {
        public Guid Id { get; set; }
        private string _data;
        public string Data//byte[]?
        {
            get => _data;
            set
            {
                _data = value;
                Size = _data.Length;
            }
        }
        public double Size { get; set; }//site of data
        //public List<string> Formats { get; set; }//fbx obj other
        public bool Textures { get; set; }
        public bool Animation { get; set; }
        public bool Rig { get; set; }
        public bool Materials { get; set; }
    }
}
