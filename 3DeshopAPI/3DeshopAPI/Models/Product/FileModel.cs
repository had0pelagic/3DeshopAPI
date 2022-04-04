namespace _3DeshopAPI.Models.Product
{
    public class FileModel
    {
        public string Name { get; set; }
        public byte[] _data;
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
        public string Format { get; set; }
    }
}
