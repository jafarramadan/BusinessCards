namespace BusinessCard.DTOs
{
    public class ImportResultDto
    {
        public List<CardDto> CreatedCards { get; set; } = new();
        public int TotalCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}


