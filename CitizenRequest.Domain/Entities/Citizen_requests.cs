using CitizenRequest.Domain.Interfaces;

namespace CitizenRequest.Domain.Entities
{
    public class Citizen_requests : IEntity<int>
    {
        public int Id { get; set; }
        public string Sender_name { get; set; }
        public string Sender_contact{ get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
        public int Channel_id { get; set; }
        public int Worker_id { get; set; }
        public virtual Channels Channel { get; set; }
        public virtual Workers Worker { get; set; }
        public virtual List<Files> File { get; set; }
    }
}
