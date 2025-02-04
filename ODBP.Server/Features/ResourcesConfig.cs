namespace ODBP.Features
{
    public class ResourcesConfig
    {
        public string? GemeenteWebsite { get; set; }
        public string? GemeenteLogo { get; set; }
        public string? GemeenteFavicon { get; set; }
        public string? GemeenteMainImage { get; set; }
        public string? GemeenteDesignTokens { get; set; }
        public string? GemeenteTheme { get; set; }

        public string? Website => Uri.TryCreate(GemeenteWebsite, UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? Logo => Uri.TryCreate(GemeenteLogo, UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? Favicon => Uri.TryCreate(GemeenteFavicon, UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? Image => Uri.TryCreate(GemeenteMainImage, UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? Tokens => Uri.TryCreate(GemeenteDesignTokens, UriKind.Absolute, out var uri) ? uri.ToString() : null;
        public string? Theme => !string.IsNullOrWhiteSpace(GemeenteTheme) ? GemeenteTheme : null;

        public void SetValues(IConfiguration config)
        {
            GemeenteWebsite = config["RESOURCES:GEMEENTE_WEBSITE"];
            GemeenteLogo = config["RESOURCES:GEMEENTE_LOGO"];
            GemeenteFavicon = config["RESOURCES:GEMEENTE_FAVICON"];
            GemeenteMainImage = config["RESOURCES:GEMEENTE_MAIN_IMAGE"];
            GemeenteDesignTokens = config["RESOURCES:GEMEENTE_DESIGN_TOKENS"];
            GemeenteTheme = config["RESOURCES:GEMEENTE_THEME"];
        }
    }
}
