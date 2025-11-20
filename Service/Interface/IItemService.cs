using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Service.Interface
{
    public interface IItemService
    {
        Task<IEnumerable<Item>> GetAllItems(string? currency);
        Task<Item> GetItemById(int id, string? currency);
        Task<Item> CreateItem(Item item, string? currency);
        Task<Item> UpdateItem(int id, Item item);
        Task DeleteItem(int id);
    }
}
