using Equipment_Rental_Backend_API.Dtos.Item;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using Mapster;
using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equipment_Rental_Backend_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetItems([FromQuery] string? currency = null)
        {
            var items = await _itemService.GetAllItems(currency);

            var itemDtos = items.Adapt<IEnumerable<ItemDto>>();
            return Ok(itemDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(int id, [FromQuery] string? currency = null)
        {
            var item = await _itemService.GetItemById(id, currency);
            if (item == null)
            {
                return NotFound();
            }
            var itemDto = item.Adapt<ItemDto>();
            return Ok(itemDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, UpdateItemDto itemDto)
        {
            var updatedItem = itemDto.Adapt<Item>();
            return Ok(await _itemService.UpdateItem(id, updatedItem));
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(CreateItemDto createItemDto, [FromQuery] string? currency = null)
        {
            var createdItem = await _itemService.CreateItem(createItemDto.Adapt<Item>(), currency);
            return CreatedAtAction(nameof(GetItems), new { /* id = createdItem.Id */ }, null);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            await _itemService.DeleteItem(id);
            return NoContent();
        }
    }
}
