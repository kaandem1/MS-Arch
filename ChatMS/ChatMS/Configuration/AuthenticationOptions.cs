namespace ChatMS.Configuration
{
    public class AuthenticationOptions
    {
        public const string Auth = "Authentication";

        public string ValidIssuer { get; set; } = string.Empty;
        public string ValidAudience { get; set; } = string.Empty;
        public string IssuerSecurityKey { get; set; } = string.Empty;



    }
}
