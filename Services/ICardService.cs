using BusinessCard.DTOs;
using BusinessCards.DTOs;

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
    }
}