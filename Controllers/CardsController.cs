using BusinessCard.DTOs;
using BusinessCard.Services;
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
        public async Task<IActionResult> CreateOrImport([FromForm] CreateCardDto form)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (form.Mode == CreateMode.ImportFromFile)
                {
                    if (form.File == null || form.File.Length == 0)
                    {
                        return BadRequest("File is required when Mode is ImportFromFile.");
                    }

                    var allowedExtensions = new[] { ".xml", ".csv" };
                    var fileExtension = Path.GetExtension(form.File.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest("Only XML and CSV files are supported.");
                    }

                    var result = await _cardService.ImportCardsAsync(form.File);
                    return Ok(result);
                }
                else
                {
                    var createdCard = await _cardService.CreateCardAsync(form);
                    return CreatedAtAction(nameof(ViewCard), new { id = createdCard.Id }, createdCard);
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
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
        public async Task<IActionResult> ExportCard(int id)
        {
            try
            {
                var csvBytes = await _cardService.ExportSingleCardCsvAsync(id);
                var fileName = $"business_card_{id}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
                return File(csvBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}