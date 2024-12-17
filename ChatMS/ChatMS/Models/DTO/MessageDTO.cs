namespace ChatMS.Models.DTO
{
    public class MessageDTO
    {
        public string Text { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public bool IsRead { get; set; }
        public bool IsTyping { get; set; }
        public string SenderName { get; set; }
    }
}
