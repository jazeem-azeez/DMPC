namespace DMC.Models
{
    public class RabbitConnectionConfiguration
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public bool IsSSlEnabled {get;set;}
        public string VirtualHost { get; set; }
    }
}