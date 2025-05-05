namespace SCMM.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<SupportTicket> SupportTickets { get; set; }

    }
}
