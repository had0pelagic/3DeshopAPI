namespace _3DeshopAPI.Models.Settings
{
    public class SMTPSettings
    {
        public string ServerAddress { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TLSPort { get; set; }
        public string SSLPort { get; set; }
    }
}
