using Anis.Template.Domain.Entities;
using Anis.Template.Test.Fakers;

namespace Anis.Template.Test.Fakers
{
    public sealed class CardDataFaker : PrivateFaker<Card>
    {
        public CardDataFaker()
        {
            UsePrivateConstructor();

            RuleFor(r => r.Id, f => f.Random.Guid());
            RuleFor(r => r.ArabicName, f => f.Random.AlphaNumeric(10));
            RuleFor(r => r.EnglishName, f => f.Random.AlphaNumeric(10));
            RuleFor(r => r.SubcategoryId, f => f.Random.Guid());
        }
    }
}