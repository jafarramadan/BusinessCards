using BusinessCard.DTOs;
using BusinessCard.Models;
using BusinessCard.Repositories;
using BusinessCards.DTOs;

namespace BusinessCard.Services
{
    public class CardService : ICardService
    {
        private readonly IRepository<Card> _cardRepository;

        public CardService(IRepository<Card> cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public async Task<IEnumerable<CardDto>> GetAllCardsAsync()
        {
            try
            {
                var cards = await _cardRepository.GetAllAsync();
                return cards.Select(card => new CardDto
                {
                    Id = card.Id,
                    Name = card.Name,
                    Gender = card.Gender,
                    DateOfBirth = card.DateOfBirth,
                    Email = card.Email,
                    Phone = card.Phone,
                    Image = card.Image,
                    Address = card.Address,
                    CreatedAt = card.CreatedAt
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all cards: {ex.Message}", ex);
            }
        }

        public async Task<CardDto?> GetCardByIdAsync(int id)
        {
            try
            {
                var card = await _cardRepository.GetByIdAsync(id);
                if (card == null)
                    return null;

                return new CardDto
                {
                    Id = card.Id,
                    Name = card.Name,
                    Gender = card.Gender,
                    DateOfBirth = card.DateOfBirth,
                    Email = card.Email,
                    Phone = card.Phone,
                    Image = card.Image,
                    Address = card.Address,
                    CreatedAt = card.CreatedAt
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving card with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<CardDto> CreateCardAsync(CreateCardDto createCardDto)
        {
            try
            {
                if (!string.IsNullOrEmpty(createCardDto.Image))
                {
                    ValidateImageSize(createCardDto.Image);
                }

                var card = new Card
                {
                    Name = createCardDto.Name,
                    Gender = createCardDto.Gender,
                    DateOfBirth = createCardDto.DateOfBirth,
                    Email = createCardDto.Email,
                    Phone = createCardDto.Phone,
                    Image = createCardDto.Image,
                    Address = createCardDto.Address
                };
                var createdCard = await _cardRepository.AddAsync(card);
                return new CardDto
                {
                    Id = createdCard.Id,
                    Name = createdCard.Name,
                    Gender = createdCard.Gender,
                    DateOfBirth = createdCard.DateOfBirth,
                    Email = createdCard.Email,
                    Phone = createdCard.Phone,
                    Image = createdCard.Image,
                    Address = createdCard.Address,
                    CreatedAt = createdCard.CreatedAt
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating card: {ex.Message}", ex);
            }
        }

        public async Task<ExportCardDto?> ExportCardAsync(int id)
        {
            try
            {
                var card = await _cardRepository.GetByIdAsync(id);
                if (card == null)
                    return null;

                return new ExportCardDto
                {
                    Id = card.Id,
                    Name = card.Name,
                    Gender = card.Gender,
                    DateOfBirth = card.DateOfBirth,
                    Email = card.Email,
                    Phone = card.Phone,
                    Image = card.Image,
                    Address = card.Address,
                    CreatedAt = card.CreatedAt
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting card with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteCardAsync(int id)
        {
            try
            {
                return await _cardRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting card with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<bool> CardExistsAsync(int id)
        {
            try
            {
                return await _cardRepository.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking if card with ID {id} exists: {ex.Message}", ex);
            }
        }

        private void ValidateImageSize(string base64Image)
        {
            try
            {
                var base64String = base64Image.Contains(',')
                    ? base64Image.Split(',')[1]
                    : base64Image;

                var sizeInBytes = (base64String.Length * 3) / 4;
                var maxSizeInBytes = 1024 * 1024; 

                if (sizeInBytes > maxSizeInBytes)
                {
                    throw new ArgumentException("Image size exceeds the maximum limit of 1MB.");
                }
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error validating image size: {ex.Message}", ex);
            }
        }
    }
}