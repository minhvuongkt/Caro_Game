namespace Client.Models
{
    public class RoomAction
    {
        public string Action { get; set; }
        public string RoomID { get; set; }
        public string TargetUID { get; set; }
        public string GameType { get; set; }
        public string RoomName { get; set; }
    }

    public class RoomInviteData
    {
        public string FromUID { get; set; }
        public string RoomID { get; set; }
        public string RoomName { get; set; }
        public string GameType { get; set; }
    }
}
