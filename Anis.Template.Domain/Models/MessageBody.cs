using System;
using MediatR;

namespace Anis.Template.Domain.Models
{
    public class MessageBody<T> : IRequest<bool> //: IMessageBody<T>
    {
        public Guid AggregateId { get; set; }
        public long Sequence { get; set; }
        public T Data { get; set; }
        public string Type { get; set; }
        public DateTime DateTime { get; set; }
        public int? Version { get; set; }

    }
}
