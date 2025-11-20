using Domain.Models;
using Mapster;
using Repository.Data;
using Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public class ItemRepository : IItemRepository
    {
        private readonly AppDbContext _context;

        public ItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Item> CreateItem(Item item)
        {
            var itemEntity = item.Adapt<ItemEntity>();
            await _context.Items.AddAsync(itemEntity);
            await _context.SaveChangesAsync();
            return itemEntity.Adapt<Item>();
        }

        public async Task DeleteItem(int id)
        {
            var itemEntity = await _context.Items.FindAsync(id);
            if (itemEntity != null)
            {
                _context.Items.Remove(itemEntity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Item>> GetAllItems()
        {
            var items = await _context.Items.ToListAsync();
            return items.Adapt<List<Item>>();
        }

        public async Task<Item> GetItemById(int id)
        {
            var itemEntity = await _context.Items.FindAsync(id);
            return itemEntity.Adapt<Item>();
        }

        public async Task<Item> UpdateItem(int id, Item item)
        {
            var existingItem = await _context.Items.FindAsync(id);
            item.Adapt(existingItem);
            await _context.SaveChangesAsync();
            return existingItem.Adapt<Item>();
        }
    }
}
