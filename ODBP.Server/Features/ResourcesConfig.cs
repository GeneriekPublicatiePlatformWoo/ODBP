namespace ODBP.Features
{
    public class ResourcesConfig(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        public string? Name => !string.IsNullOrWhiteSpace(_configuration["RESOURCES:GEMEENTE_NAAM"]) ? _configuration["RESOURCES:GEMEENTE_NAAM"] : null;
        public string? LogoUrl => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_LOGO_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? FaviconUrl => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_FAVICON_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? ImageUrl => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_MAIN_IMAGE_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? TokensUrl => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_DESIGN_TOKENS_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? Theme => !string.IsNullOrWhiteSpace(_configuration["RESOURCES:GEMEENTE_THEME_NAAM"]) ? _configuration["RESOURCES:GEMEENTE_THEME_NAAM"] : null;
        public string? WebsiteUrl => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_WEBSITE_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? PrivacyUrl => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_PRIVACY_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? ContactUrl => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_CONTACT_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? A11yUrl => Uri.TryCreate(_configuration["RESOURCES:TOEGANKELIJKHEIDSVERKLARING_REGISTER_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
    }
}
