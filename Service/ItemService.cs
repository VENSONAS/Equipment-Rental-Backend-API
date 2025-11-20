using Domain.Models;
using Integration;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Logging;
using Repository;
using Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly ICurrencyService _currencyService;
        private readonly ILogger<ItemService> _logger;

        public ItemService(IItemRepository itemRepository, ICurrencyService currencyService, ILogger<ItemService> logger)
        {
            _itemRepository = itemRepository;
            _currencyService = currencyService;
            _logger = logger;
        }

        public async Task<Item> CreateItem(Item item, string? currency)
        {
            var itemToCreate = await _itemRepository.CreateItem(item);

            if (currency != null)
            {
                var conversionRate = await _currencyService.GetExchangeFromAsync(currency);
                itemToCreate.ConvertBaseDailyPrice(conversionRate);
            }
            itemToCreate.SetCreatedTime();
            _logger.LogInformation($"Booking created with ID {itemToCreate.Id}");
            return itemToCreate;
        }

        public async Task DeleteItem(int id)
        {
            await _itemRepository.DeleteItem(id);
        }

        public async Task<IEnumerable<Item>> GetAllItems(string? currency)
        { 
            var items = await _itemRepository.GetAllItems();
            if (currency == null) return items;

            var conversionRate = await _currencyService.GetExchangeToAsync(currency);

            foreach (var item in items)
            {
                item.ConvertBaseDailyPrice(conversionRate);
            }

            return items;
        }


        public async Task<Item> GetItemById(int id, string? currency)
        {
            if (currency != null)
            {
                var item =  await _itemRepository.GetItemById(id);
                var conversionRate = await _currencyService.GetExchangeToAsync(currency);
                item.ConvertBaseDailyPrice(conversionRate);
                return item;
            }
            return await _itemRepository.GetItemById(id);
        }

        public async Task<Item> UpdateItem(int id, Item item)
        {
            var itemToUpdate = await _itemRepository.GetItemById(id);

            itemToUpdate.ChangeName(item.Name);
            itemToUpdate.ChangeCategory(item.Category);
            itemToUpdate.ChangeBaseDailyPrice(item.BaseDailyPrice);
            itemToUpdate.ChangeSecurityDeposit(item.SecurityDeposit);
            itemToUpdate.ChangeTotalStock(item.TotalStock);
            itemToUpdate.SetActiveStatus(item.Active);

            return await _itemRepository.UpdateItem(id, itemToUpdate);
        }
    }
}
