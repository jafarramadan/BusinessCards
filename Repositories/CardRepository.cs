using BusinessCard.Data;
using BusinessCard.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessCard.Repositories
{
    public class CardRepository : IRepository<Card>
    {
        private readonly BusinessCardsDbContext _context;

        public CardRepository(BusinessCardsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Card>> GetAllAsync()
        {
            return await _context.Cards
                .ToListAsync();
        }

        public async Task<Card?> GetByIdAsync(int id)
        {
            return await _context.Cards
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Card> AddAsync(Card entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsDeleted = false;

            _context.Cards.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Card> UpdateAsync(Card entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Cards
                .IgnoreQueryFilters() 
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity == null || entity.IsDeleted)
                return false;

            entity.IsDeleted = true;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Cards.AnyAsync(e => e.Id == id);
        }
    }
}