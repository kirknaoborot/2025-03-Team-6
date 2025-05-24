using CitizenRequest.Domain.Interfaces;

namespace CitizenRequest.Domain.Entities
{
    public class Channel_tip : IEntity<int>
    {
        public int Id { get; set; }
        public string Tip { get; set; }
        public virtual List<Channels> Tips { get; set; }
    }
}
