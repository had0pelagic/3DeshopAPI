﻿namespace _3DeshopAPI.Models.Product
{
    public class ProductSpecificationsModel
    {
        private string _data;
        public string Data//byte[]
        {
            get => _data;
            set
            {
                _data = value;
                Size = _data.Length;
            }
        }
        public double Size { get; set; }
        public bool Textures { get; set; }
        public bool Animation { get; set; }
        public bool Rig { get; set; }
        public bool Materials { get; set; }
    }
}