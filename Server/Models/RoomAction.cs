namespace Server.Models
{
    // Action values: "Create" | "Join" | "Spectate" | "Leave" | "Invite" |
    //                "AcceptInvite" | "DeclineInvite" | "List"
    public class RoomAction
    {
        public string Action { get; set; }
        public string RoomID { get; set; }
        public string TargetUID { get; set; }   // used by Invite
        public string GameType { get; set; }    // used by Create: "Caro" | "Go" | "TwoVsTwo"
        public string RoomName { get; set; }    // used by Create
    }

    // Payload sent to an invited player
    public class RoomInviteData
    {
        public string FromUID { get; set; }
        public string RoomID { get; set; }
        public string RoomName { get; set; }
        public string GameType { get; set; }
    }
}
