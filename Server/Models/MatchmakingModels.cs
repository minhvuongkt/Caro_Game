namespace Server.Models
{
    /// <summary>
    /// Sent by the server to both players when a random-match pair is found.
    /// Each recipient should respond with MatchAccept or MatchDecline within the countdown window.
    /// </summary>
    public class MatchFoundData
    {
        /// <summary>Unique identifier for this pending match (echoed back in Accept / Decline).</summary>
        public string MatchID      { get; set; }

        /// <summary>UID of the opponent.</summary>
        public string OpponentUID  { get; set; }

        /// <summary>Display name of the opponent (fetched from Players table).</summary>
        public string OpponentName { get; set; }

        /// <summary>Game type string: "Caro" | "Go" | "TwoVsTwo".</summary>
        public string GameType     { get; set; }
    }
}
