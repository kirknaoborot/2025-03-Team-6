using CitizenRequest.Domain.Interfaces;

namespace CitizenRequest.Domain.Entities
{
    public class Files : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public int Request_id { get; set; }
        public virtual Citizen_requests Citizen_req { get; set; }
        public string FileId { get; set; }
    }
}
