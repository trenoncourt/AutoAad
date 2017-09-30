namespace AutoAad.Api
{
    public class AppSettings
    {
        public CorsSettings Cors { get; set; }

        public AzureAdSettings AzureAd { get; set; }
    }

    public class AzureAdSettings
    {
        public string Instance { get; set; }

        public string Domain { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string GraphVersion { get; set; }

        public string Resource { get; set; }
    }

    public class CorsSettings
    {
        public bool Enabled { get; set; }

        public string Methods { get; set; }

        public string Origins { get; set; }

        public string Headers { get; set; }
    }
}