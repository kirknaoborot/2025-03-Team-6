using CitizenRequest.Domain.Interfaces;

namespace CitizenRequest.Domain.Entities
{
    public class Channels : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public string Port { get; set; }
        public string Token { get; set; }
        public int Channel_tip_id { get; set; }
        public string Is_activ { get; set; }
        public virtual Channel_tip Chan_tip { get; set; }
        public virtual List<Citizen_requests> Citizen_req { get; set; }

    }
}
