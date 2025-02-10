namespace ODBP.Features.Publicaties
{
    public class Publicatie
    {
        public Guid Uuid { get; set; }
        public string? Publisher { get; set; }
        public string? Verantwoordelijke { get; set; }
        public string? OfficieleTitel { get; set; }
        public string? VerkorteTitel { get; set; }
        public string? Omschrijving { get; set; }
        public Eigenaar? Eigenaar { get; set; }
        public string? Publicatiestatus { get; set; }
        public DateTime Registratiedatum { get; set; }
        public DateTime LaatstGewijzigdDatum { get; set; }
        public List<string>? InformatieCategorieen { get; set; }
    }

    public class Eigenaar
    {
        public string? Identifier { get; set; }
        public string? WeergaveNaam { get; set; }
    }
}
