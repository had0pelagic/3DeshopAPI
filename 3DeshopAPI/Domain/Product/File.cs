namespace Domain.Product
{
    public class File
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
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
        public double Size { get; set; }
        public string Format { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
