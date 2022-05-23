namespace _3DeshopAPI.Models;

public class ImageModel
{
    public string Name { get; set; }
    public byte[] Data { get; set; }
    public double? Size { get; set; }
    public string Format { get; set; }
}