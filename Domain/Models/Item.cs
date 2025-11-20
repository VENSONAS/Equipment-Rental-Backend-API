using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Item
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Category { get; private set; }

        private decimal _baseDailyPrice;
        public decimal BaseDailyPrice
        {
            get => Math.Round(_baseDailyPrice, 2, MidpointRounding.AwayFromZero);
            private set => _baseDailyPrice = value;
        }
        public decimal SecurityDeposit { get; private set; }
        public int TotalStock { get; private set; }
        public bool Active { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Item() { }

        public void ConvertBaseDailyPrice(decimal conversionRate)
        {
            BaseDailyPrice *= conversionRate;
        }

        public void SetCreatedTime()
        {
            CreatedAt = DateTime.UtcNow;
        }
        public void ChangeName(string newName)
        {
            Name = newName;
        }
        public void ChangeCategory(string newCategory)
        {
            Category = newCategory;
        }
        public void ChangeBaseDailyPrice(decimal newPrice)
        {
            BaseDailyPrice = newPrice;
        }
        public void ChangeSecurityDeposit(decimal newDeposit)
        {
            SecurityDeposit = newDeposit;
        }
        public void ChangeTotalStock(int newStock)
        {
            TotalStock = newStock;
        }
        public void SetActiveStatus(bool isActive)
        {
            Active = isActive;
        }
    }
}
