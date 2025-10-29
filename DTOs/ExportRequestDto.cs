namespace BusinessCard.DTOs
{
    public class ExportRequestDto
    {
        public List<int> CardIds { get; set; } = new();
        public string Format { get; set; } = "xml"; 
    }
}


