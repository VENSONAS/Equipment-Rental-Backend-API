namespace Equipment_Rental_Backend_API.Dtos.Item
{
    public class ItemDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = default!;
        public string Category { get; init; } = default!;
        public decimal BaseDailyPrice { get; init; }
        public decimal SecurityDeposit { get; init; }
        public int TotalStock { get; init; }
        public bool Active { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    public class CreateItemDto
    {
        public string Name { get; init; } = default!;
        public string Category { get; init; } = default!;
        public decimal BaseDailyPrice { get; init; }
        public decimal SecurityDeposit { get; init; }
        public int TotalStock { get; init; }
    }

    public class UpdateItemDto
    {
        public string Name { get; init; } = default!;
        public string Category { get; init; } = default!;
        public decimal BaseDailyPrice { get; init; }
        public decimal SecurityDeposit { get; init; }
        public int TotalStock { get; init; }
        public bool Active { get; init; }
    }
}
