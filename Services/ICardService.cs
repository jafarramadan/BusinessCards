using BusinessCard.DTOs;

namespace BusinessCard.Services
{
    public interface ICardService
    {
        Task<IEnumerable<CardDto>> GetAllCardsAsync();
        Task<CardDto?> GetCardByIdAsync(int id);
        Task<CardDto> CreateCardAsync(CreateCardDto createCardDto);
        Task<bool> DeleteCardAsync(int id);
        Task<ExportCardDto?> ExportCardAsync(int id);
        Task<bool> CardExistsAsync(int id);
        
        Task<ImportResultDto> ImportCardsAsync(IFormFile file);
        Task<byte[]> ExportCardsAsync(ExportRequestDto exportRequest);

        Task<byte[]> ExportSingleCardCsvAsync(int id);
    }
}