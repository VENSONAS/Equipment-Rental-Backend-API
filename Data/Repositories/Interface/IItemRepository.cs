using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAllItems();
        Task<Item> GetItemById(int id);
        Task<Item> CreateItem(Item item);
        Task<Item> UpdateItem(int id, Item item);
        Task DeleteItem(int id);
    }
}
