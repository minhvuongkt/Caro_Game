namespace Server.Models
{
    // Room actions:  "Create" | "Join" | "Spectate" | "Leave" | "Invite" |
    //                "AcceptInvite" | "DeclineInvite" | "List"
    // Matchmaking:   "MatchQueue" | "MatchCancel" | "MatchAccept" | "MatchDecline"
    public class RoomAction
    {
        public string Action { get; set; }
        public string RoomID { get; set; }      // room ID; also used as MatchID for matchmaking responses
        public string TargetUID { get; set; }   // used by Invite
        public string GameType { get; set; }    // used by Create / MatchQueue: "Caro" | "Go" | "TwoVsTwo"
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
