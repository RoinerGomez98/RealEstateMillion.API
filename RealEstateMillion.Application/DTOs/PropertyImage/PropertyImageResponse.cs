namespace RealEstateMillion.Application.DTOs.PropertyImage
{
    public class PropertyImageResponse
    {
        public Guid Id { get; set; }
        public string File { get; set; } = null!;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPrimary { get; set; }
        public bool Enabled { get; set; }
        public string? FileType { get; set; }
        public long? FileSizeBytes { get; set; }
        public string? ThumbnailPath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
