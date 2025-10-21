namespace mails
{
    public class MailListItem
    {
        public string Id { get; set; } = "";
        public string Display { get; set; } = "";

        public override string ToString() => Display;
    }
}