namespace ODBP.Features
{
    public class ResourcesConfig(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        public string? Name => !string.IsNullOrWhiteSpace(_configuration["RESOURCES:GEMEENTE_NAAM"]) ? _configuration["RESOURCES:GEMEENTE_NAAM"] : null;
        public string? Website => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_WEBSITE_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? Logo => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_LOGO_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? Favicon => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_FAVICON_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? Image => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_MAIN_IMAGE_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? Tokens => Uri.TryCreate(_configuration["RESOURCES:GEMEENTE_DESIGN_TOKENS_URL"], UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? Theme => !string.IsNullOrWhiteSpace(_configuration["RESOURCES:GEMEENTE_THEME_NAAM"]) ? _configuration["RESOURCES:GEMEENTE_THEME_NAAM"] : null;
    }
}
