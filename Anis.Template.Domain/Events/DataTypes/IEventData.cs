using System.Text.Json.Serialization;
using Anis.Template.Domain.Enums;

namespace Anis.Template.Domain.Events.DataTypes
{
    public interface IEventData
    {
        [JsonIgnore]
        EventTypes Type { get; }
    }
}
