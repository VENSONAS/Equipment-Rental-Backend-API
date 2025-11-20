namespace Repository.Entities
{
    public class ItemEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Category { get; set; }
        public decimal BaseDailyPrice { get; set; }
        public decimal SecurityDeposit { get; set; }
        public int TotalStock { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
