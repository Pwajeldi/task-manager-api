namespace Task_Management_App.Models
{
    public class EmailRequestMod
    {
        public required string receiverName { get; set; }
        public required string receiverEmail { get; set; }
        public required string subject { get; set; }
        public required string content { get; set; }
    }
}
