namespace Equipment_Rental_Backend_API.Dtos.Response
{
    public class CurrencyExchangeInfo
    {
        public required string FromCurrency { get; set; }
        public required string ToCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}
