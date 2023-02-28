using Anis.Template.Domain.Events.DataTypes;
using Anis.Template.Domain.Events.DataTypes;
using Anis.Template.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Anis.Template.Domain.Entities
{
    public class Card
    {
        private Card()
        {
        }

        public Card(MessageBody<CardCreatedData> model)
        {
            Id = model.AggregateId;
            Sequence = 1;
            ArabicName = model.Data.ArabicName;
            EnglishName = model.Data.EnglishName;
            SubcategoryId = model.Data.SubcategoryId;
        }

        public Guid Id { get; private set; }
        public string ArabicName { get; private set; }
        public string EnglishName { get; private set; }
        public Guid SubcategoryId { get; private set; }
        public long Sequence { get; private set; }


        public void Create(MessageBody<CardCreatedData> model)
        {
            Sequence = model.Sequence;
            ArabicName = model.Data.ArabicName;
            EnglishName = model.Data.EnglishName;
            SubcategoryId = model.Data.SubcategoryId;

        }

        public void IncrementSequence()
        {
            ++Sequence;
        }
    }
}
