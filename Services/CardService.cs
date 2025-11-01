using BusinessCard.DTOs;
using BusinessCard.Models;
using BusinessCard.Repositories;
using System.Text;
using System.Xml;
using System.Globalization;

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
                    DateOfBirth = createCardDto.DateOfBirth.Value,
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

        public async Task<CardDto?> ExportCardAsync(int id)
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

        public async Task<ImportResultDto> ImportCardsAsync(IFormFile file)
        {
            try
            {
                var cardImports = new List<CardImportDto>();
                var errors = new List<string>();
                var createdCards = new List<CardDto>();

                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);

                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                switch (fileExtension)
                {
                    case ".xml":
                        ParseXmlFile(reader, cardImports, errors);
                        break;
                    case ".csv":
                        ParseCsvFile(reader, cardImports, errors);
                        break;
                    default:
                        errors.Add($"Unsupported file format: {fileExtension}. Only XML and CSV files are supported.");
                        break;
                }

                if (errors.Any())
                {
                    return new ImportResultDto
                    {
                        CreatedCards = createdCards,
                        TotalCount = 0,
                        Errors = errors
                    };
                }

                foreach (var cardImport in cardImports)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(cardImport.Image))
                        {
                            ValidateImageSize(cardImport.Image);
                        }

                        var card = new Card
                        {
                            Name = cardImport.Name,
                            Gender = cardImport.Gender,
                            DateOfBirth = cardImport.DateOfBirth,
                            Email = cardImport.Email,
                            Phone = cardImport.Phone,
                            Image = cardImport.Image,
                            Address = cardImport.Address
                        };

                        var createdCard = await _cardRepository.AddAsync(card);
                        createdCards.Add(new CardDto
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
                        });
                    }
                    catch (ArgumentException ex)
                    {
                        errors.Add($"Error creating card '{cardImport.Name}': {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error creating card '{cardImport.Name}': {ex.Message}");
                    }
                }

                return new ImportResultDto
                {
                    CreatedCards = createdCards,
                    TotalCount = createdCards.Count,
                    Errors = errors
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error importing file: {ex.Message}", ex);
            }
        }


        public async Task<byte[]> ExportSingleCardCsvAsync(int id)
        {
            try
            {
                var dto = await ExportCardAsync(id);
                if (dto == null)
                {
                    throw new ArgumentException($"Card with ID {id} not found.");
                }

                return GenerateCsvExportFromDtos(new List<CardDto> { dto });
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error exporting card CSV: {ex.Message}", ex);
            }
        }

        private void ParseXmlFile(StreamReader reader, List<CardImportDto> cards, List<string> errors)
        {
            try
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(reader);

                var cardNodes = xmlDoc.SelectNodes("//Card");
                if (cardNodes == null)
                {
                    errors.Add("No Card elements found in XML file.");
                    return;
                }

                foreach (XmlNode cardNode in cardNodes)
                {
                    try
                    {
                        var card = new CardImportDto
                        {
                            Name = GetXmlValue(cardNode, "Name"),
                            Gender = GetXmlValue(cardNode, "Gender"),
                            DateOfBirth = DateTime.Parse(GetXmlValue(cardNode, "DateOfBirth")),
                            Email = GetXmlValue(cardNode, "Email"),
                            Phone = GetXmlValue(cardNode, "Phone"),
                            Image = GetXmlValue(cardNode, "Image"),
                            Address = GetXmlValue(cardNode, "Address")
                        };

                        cards.Add(card);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error parsing card: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Error parsing XML file: {ex.Message}");
            }
        }

        private void ParseCsvFile(StreamReader reader, List<CardImportDto> cards, List<string> errors)
        {
            try
            {
                var lines = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        lines.Add(line);
                    }
                }

                if (lines.Count < 2)
                {
                    errors.Add("CSV file must contain at least a header row and one data row.");
                    return;
                }

                var headers = ParseCsvLine(lines[0]).Select(h => h.Trim().ToLowerInvariant()).ToArray();
                var requiredHeaders = new[] { "name", "gender", "dateofbirth", "email", "phone", "address" };

                foreach (var required in requiredHeaders)
                {
                    if (!headers.Contains(required))
                    {
                        errors.Add($"Missing required column: {required}");
                    }
                }

                if (errors.Any()) return;

                for (int i = 1; i < lines.Count; i++)
                {
                    try
                    {
                        var values = ParseCsvLine(lines[i]);
                        if (values.Length != headers.Length)
                        {
                            errors.Add($"Row {i + 1}: Column count mismatch. Expected {headers.Length}, got {values.Length}");
                            continue;
                        }

                        var card = new CardImportDto
                        {
                            Name = GetCsvValue(values, headers, "name"),
                            Gender = GetCsvValue(values, headers, "gender"),
                            DateOfBirth = ParseDate(GetCsvValue(values, headers, "dateofbirth")),
                            Email = GetCsvValue(values, headers, "email"),
                            Phone = ParsePhone(GetCsvValue(values, headers, "phone")),
                            Image = GetCsvValue(values, headers, "image"),
                            Address = GetCsvValue(values, headers, "address")
                        };

                        cards.Add(card);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {i + 1}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Error parsing CSV file: {ex.Message}");
            }
        }

        private string GetXmlValue(XmlNode node, string elementName)
        {
            return node.SelectSingleNode(elementName)?.InnerText?.Trim() ?? "";
        }

        private string GetCsvValue(string[] values, string[] headers, string columnName)
        {
            var index = Array.IndexOf(headers, columnName);
            return index >= 0 && index < values.Length ? values[index].Trim() : "";
        }

        private DateTime ParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                throw new ArgumentException("Date cannot be empty");

            var normalizedDate = dateString.Trim();
            
            var formats = new[]
            {
                "yyyy-M-d",     
                "yyyy-MM-dd",   
                "M/d/yyyy",      
                "MM/dd/yyyy",   
                "yyyy-MM-ddTHH:mm:ss", 
                "yyyy-MM-ddTHH:mm:ss.fffffffZ" 
            };

            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(normalizedDate, format, null, System.Globalization.DateTimeStyles.None, out var result))
                {
                    return result;
                }
            }

            if (DateTime.TryParse(normalizedDate, out var fallbackResult))
            {
                return fallbackResult;
            }

            throw new ArgumentException($"Unable to parse date: {dateString}");
        }

        private string ParsePhone(string phoneString)
        {
            if (string.IsNullOrWhiteSpace(phoneString))
                return "";

            var phone = phoneString.Trim();

            if (phone.Contains("E+") || phone.Contains("e+"))
            {
                if (double.TryParse(phone, System.Globalization.NumberStyles.Float, null, out var scientificValue))
                {
                    var phoneNumber = ((long)scientificValue).ToString();
                    return phoneNumber;
                }
            }

            return phone;
        }

        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var current = new StringBuilder();
            var inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            result.Add(current.ToString());
            return result.ToArray();
        }

        private byte[] GenerateCsvExportFromDtos(List<CardDto> cards)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Id,Name,Gender,DateOfBirth,Email,Phone,Address,Image,CreatedAt");

            foreach (var card in cards)
            {
                csv.AppendLine($"{card.Id},{EscapeCsvValue(card.Name)},{EscapeCsvValue(card.Gender)},{card.DateOfBirth:yyyy-MM-dd},{EscapeCsvValue(card.Email)},{EscapeCsvValue(card.Phone)},{EscapeCsvValue(card.Address)},{EscapeCsvValue(card.Image ?? "")},{card.CreatedAt:yyyy-MM-ddTHH:mm:ssZ}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        private string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }
    }
}