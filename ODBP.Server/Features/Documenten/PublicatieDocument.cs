namespace ODBP.Features.Documenten
{
    public class PublicatieDocument
    {
        public Guid Uuid { get; set; }
        public string? Identifier { get; set; }
        public Guid Publicatie { get; set; }
        public required string OfficieleTitel { get; set; }
        public string? VerkorteTitel { get; set; }
        public string? Omschrijving { get; set; }
        public Eigenaar? Eigenaar { get; set; }
        public string? Publicatiestatus { get; set; }
        public DateOnly Creatiedatum { get; set; }
        public DateTime Registratiedatum { get; set; }
        public DateTime LaatstGewijzigdDatum { get; set; }
        public required string Bestandsnaam { get; set; }
        public required string Bestandsformaat { get; set; }
        public required double Bestandsomvang { get; set; }
    }

    public class Eigenaar
    {
        public string? Identifier { get; set; }
        public string? WeergaveNaam { get; set; }
    }
}
