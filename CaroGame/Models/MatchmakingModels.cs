namespace Client.Models
{
    /// <summary>
    /// Sent by the server to both players when a random-match pair is found.
    /// The client should show an accept/decline dialog within the countdown window.
    /// </summary>
    public class MatchFoundData
    {
        /// <summary>Unique identifier for this pending match (used in Accept / Decline responses).</summary>
        public string MatchID      { get; set; }

        /// <summary>UID of the opponent.</summary>
        public string OpponentUID  { get; set; }

        /// <summary>Display name of the opponent.</summary>
        public string OpponentName { get; set; }

        /// <summary>Game type string: "Caro" | "Go" | "TwoVsTwo".</summary>
        public string GameType     { get; set; }
    }
}
