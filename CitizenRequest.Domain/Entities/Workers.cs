using CitizenRequest.Domain.Interfaces;

namespace CitizenRequest.Domain.Entities
{
    public class Workers : IEntity<int>
    {
        public int Id { get; set; }
        public string Full_name { get; set; }
        public string Login { get; set; }
        public string Password_hash { get; set; }
        public string Is_active { get; set; }
        public string Status { get; set; }
        public DateTime? Last_seen { get; set; }
        public string Role { get; set; }
        public virtual List<Citizen_requests> Citizen_req { get; set; }

    }
}
