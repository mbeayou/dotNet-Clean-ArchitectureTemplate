namespace Anis.StockTracker.Domain.Events.DataTypes
{
    public record CardCreatedData(string ArabicName, string EnglishName, Guid SubcategoryId)
    {
        public CardCreatedData() : this(default, default, default)
        {
        }
    }
}