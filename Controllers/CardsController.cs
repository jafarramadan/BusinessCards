using BusinessCard.DTOs;
using BusinessCard.Services;
using BusinessCards.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BusinessCard.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardController : ControllerBase
    {
        private readonly ICardService _cardService;

        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CardDto>>> ViewCards()
        {
            var cards = await _cardService.GetAllCardsAsync();
            return Ok(cards);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CardDto>> ViewCard(int id)
        {
            var card = await _cardService.GetCardByIdAsync(id);

            if (card == null)
            {
                return NotFound($"Card with ID {id} not found.");
            }

            return Ok(card);
        }

        [HttpPost]
        public async Task<ActionResult<CardDto>> CreateCard(CreateCardDto createCardDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdCard = await _cardService.CreateCardAsync(createCardDto);
                return CreatedAtAction(nameof(ViewCard), new { id = createdCard.Id }, createdCard);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCard(int id)
        {
            var deleted = await _cardService.DeleteCardAsync(id);

            if (!deleted)
            {
                return NotFound($"Card with ID {id} not found.");
            }

            return NoContent();
        }

        [HttpGet("{id}/export")]
        public async Task<ActionResult<ExportCardDto>> ExportCard(int id)
        {
            var card = await _cardService.ExportCardAsync(id);

            if (card == null)
            {
                return NotFound($"Card with ID {id} not found.");
            }

            return Ok(card);
        }
    }
}