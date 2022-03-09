namespace _3DeshopAPI.Models.Product
{
    public class ProductSpecificationsModel
    {
        private byte[] _data;
        public byte[] Data
        {
            get => _data;
            set
            {
                _data = value;
                Size = _data.Length;
            }
        }
        public double? Size { get; set; }
        public bool Textures { get; set; }
        public bool Animation { get; set; }
        public bool Rig { get; set; }
        public bool Materials { get; set; }
    }
}
