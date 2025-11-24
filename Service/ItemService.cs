using Domain.Models;
using Integration;
using Microsoft.Extensions.Logging;
using Repository;
using Service.Interface;
using System;
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
            _itemRepository = itemRepository ?? throw new ArgumentNullException(nameof(itemRepository));
            _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Item> CreateItem(Item item, string? currency)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var itemToCreate = await _itemRepository.CreateItem(item)
                ?? throw new InvalidOperationException("Repository returned null when creating an item.");

            if (!string.IsNullOrWhiteSpace(currency))
            {
                var conversionRate = await _currencyService.GetExchangeFromAsync(currency);
                if (conversionRate == null)
                    throw new InvalidOperationException("Received null conversion rate.");

                itemToCreate.ConvertBaseDailyPrice(conversionRate);
            }

            itemToCreate.SetCreatedTime();
            _logger.LogInformation($"Booking created with ID {itemToCreate.Id}");

            return itemToCreate;
        }

        public async Task DeleteItem(int id)
        {
            if (id <= 0) throw new ArgumentException("ID must be a positive integer.", nameof(id));
            await _itemRepository.DeleteItem(id);
        }

        public async Task<IEnumerable<Item>> GetAllItems(string? currency)
        {
            var items = await _itemRepository.GetAllItems()
                ?? throw new InvalidOperationException("Repository returned null list of items.");

            if (string.IsNullOrWhiteSpace(currency)) return items;

            var conversionRate = await _currencyService.GetExchangeToAsync(currency);
            if (conversionRate == null)
                throw new InvalidOperationException("Received null conversion rate.");

            foreach (var item in items)
            {
                if (item == null) continue;
                item.ConvertBaseDailyPrice(conversionRate);
            }

            return items;
        }

        public async Task<Item> GetItemById(int id, string? currency)
        {
            if (id <= 0) throw new ArgumentException("ID must be a positive integer.", nameof(id));

            var item = await _itemRepository.GetItemById(id)
                ?? throw new InvalidOperationException($"Item with ID {id} not found.");

            if (!string.IsNullOrWhiteSpace(currency))
            {
                var conversionRate = await _currencyService.GetExchangeToAsync(currency);
                if (conversionRate == null)
                    throw new InvalidOperationException("Received null conversion rate.");

                item.ConvertBaseDailyPrice(conversionRate);
            }

            return item;
        }

        public async Task<Item> UpdateItem(int id, Item item)
        {
            if (id <= 0) throw new ArgumentException("ID must be a positive integer.", nameof(id));
            if (item == null) throw new ArgumentNullException(nameof(item));

            var itemToUpdate = await _itemRepository.GetItemById(id)
                ?? throw new InvalidOperationException($"Item with ID {id} not found.");

            if (string.IsNullOrWhiteSpace(item.Name))
                throw new ArgumentException("Item name cannot be empty.", nameof(item));
            if (item.BaseDailyPrice < 0)
                throw new ArgumentException("Base daily price cannot be negative.", nameof(item));
            if (item.TotalStock < 0)
                throw new ArgumentException("Total stock cannot be negative.", nameof(item));

            itemToUpdate.ChangeName(item.Name);
            itemToUpdate.ChangeCategory(item.Category);
            itemToUpdate.ChangeBaseDailyPrice(item.BaseDailyPrice);
            itemToUpdate.ChangeSecurityDeposit(item.SecurityDeposit);
            itemToUpdate.ChangeTotalStock(item.TotalStock);
            itemToUpdate.SetActiveStatus(item.Active);

            var updated = await _itemRepository.UpdateItem(id, itemToUpdate)
                ?? throw new InvalidOperationException("Repository returned null after update.");

            return updated;
        }
    }
}
