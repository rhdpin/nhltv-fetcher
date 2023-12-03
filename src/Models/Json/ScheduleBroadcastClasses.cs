using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NhlTvFetcher.Models.Json.ScheduleBroadcast
{
    public class AwayTeam
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("placeName")]
        public PlaceName PlaceName { get; set; }

        [JsonProperty("abbrev")]
        public string Abbrev { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("darkLogo")]
        public string DarkLogo { get; set; }

        [JsonProperty("awaySplitSquad")]
        public bool AwaySplitSquad { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("radioLink")]
        public string RadioLink { get; set; }

        [JsonProperty("odds")]
        public List<Odd> Odds { get; set; }
    }

    public class FirstInitial
    {
        [JsonProperty("default")]
        public string Default { get; set; }
    }

    public class Game
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("season")]
        public int Season { get; set; }

        [JsonProperty("gameType")]
        public int GameType { get; set; }

        [JsonProperty("venue")]
        public Venue Venue { get; set; }

        [JsonProperty("neutralSite")]
        public bool NeutralSite { get; set; }

        [JsonProperty("startTimeUTC")]
        public DateTime StartTimeUTC { get; set; }

        [JsonProperty("easternUTCOffset")]
        public string EasternUTCOffset { get; set; }

        [JsonProperty("venueUTCOffset")]
        public string VenueUTCOffset { get; set; }

        [JsonProperty("venueTimezone")]
        public string VenueTimezone { get; set; }

        [JsonProperty("gameState")]
        public string GameState { get; set; }

        [JsonProperty("gameScheduleState")]
        public string GameScheduleState { get; set; }

        [JsonProperty("tvBroadcasts")]
        public List<TvBroadcast> TvBroadcasts { get; set; }

        [JsonProperty("awayTeam")]
        public AwayTeam AwayTeam { get; set; }

        [JsonProperty("homeTeam")]
        public HomeTeam HomeTeam { get; set; }

        [JsonProperty("periodDescriptor")]
        public PeriodDescriptor PeriodDescriptor { get; set; }

        [JsonProperty("gameOutcome")]
        public GameOutcome GameOutcome { get; set; }

        [JsonProperty("winningGoalie")]
        public WinningGoalie WinningGoalie { get; set; }

        [JsonProperty("winningGoalScorer")]
        public WinningGoalScorer WinningGoalScorer { get; set; }

        [JsonProperty("threeMinRecap")]
        public string ThreeMinRecap { get; set; }

        [JsonProperty("gameCenterLink")]
        public string GameCenterLink { get; set; }

        [JsonProperty("threeMinRecapFr")]
        public string ThreeMinRecapFr { get; set; }

        [JsonProperty("ticketsLink")]
        public string TicketsLink { get; set; }
    }

    public class GameOutcome
    {
        [JsonProperty("lastPeriodType")]
        public string LastPeriodType { get; set; }
    }

    public class GameWeek
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("dayAbbrev")]
        public string DayAbbrev { get; set; }

        [JsonProperty("numberOfGames")]
        public int NumberOfGames { get; set; }

        [JsonProperty("games")]
        public List<Game> Games { get; set; }
    }

    public class HomeTeam
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("placeName")]
        public PlaceName PlaceName { get; set; }

        [JsonProperty("abbrev")]
        public string Abbrev { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("darkLogo")]
        public string DarkLogo { get; set; }

        [JsonProperty("homeSplitSquad")]
        public bool HomeSplitSquad { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("radioLink")]
        public string RadioLink { get; set; }

        [JsonProperty("odds")]
        public List<Odd> Odds { get; set; }
    }

    public class LastName
    {
        [JsonProperty("default")]
        public string Default { get; set; }

        [JsonProperty("cs")]
        public string Cs { get; set; }

        [JsonProperty("sk")]
        public string Sk { get; set; }

        [JsonProperty("fi")]
        public string Fi { get; set; }
    }

    public class Odd
    {
        [JsonProperty("providerId")]
        public int ProviderId { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class OddsPartner
    {
        [JsonProperty("partnerId")]
        public int PartnerId { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("siteUrl")]
        public string SiteUrl { get; set; }

        [JsonProperty("bgColor")]
        public string BgColor { get; set; }

        [JsonProperty("textColor")]
        public string TextColor { get; set; }

        [JsonProperty("accentColor")]
        public string AccentColor { get; set; }
    }

    public class PeriodDescriptor
    {
        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("periodType")]
        public string PeriodType { get; set; }
    }

    public class PlaceName
    {
        [JsonProperty("default")]
        public string Default { get; set; }

        [JsonProperty("fr")]
        public string Fr { get; set; }
    }

    public class RootObject
    {
        [JsonProperty("nextStartDate")]
        public string NextStartDate { get; set; }

        [JsonProperty("previousStartDate")]
        public string PreviousStartDate { get; set; }

        [JsonProperty("gameWeek")]
        public List<GameWeek> GameWeek { get; set; }

        [JsonProperty("oddsPartners")]
        public List<OddsPartner> OddsPartners { get; set; }

        [JsonProperty("preSeasonStartDate")]
        public string PreSeasonStartDate { get; set; }

        [JsonProperty("regularSeasonStartDate")]
        public string RegularSeasonStartDate { get; set; }

        [JsonProperty("regularSeasonEndDate")]
        public string RegularSeasonEndDate { get; set; }

        [JsonProperty("playoffEndDate")]
        public string PlayoffEndDate { get; set; }

        [JsonProperty("numberOfGames")]
        public int NumberOfGames { get; set; }
    }

    public class TvBroadcast
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("market")]
        public string Market { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("network")]
        public string Network { get; set; }
    }

    public class Venue
    {
        [JsonProperty("default")]
        public string Default { get; set; }
    }

    public class WinningGoalie
    {
        [JsonProperty("playerId")]
        public int PlayerId { get; set; }

        [JsonProperty("firstInitial")]
        public FirstInitial FirstInitial { get; set; }

        [JsonProperty("lastName")]
        public LastName LastName { get; set; }
    }

    public class WinningGoalScorer
    {
        [JsonProperty("playerId")]
        public int PlayerId { get; set; }

        [JsonProperty("firstInitial")]
        public FirstInitial FirstInitial { get; set; }

        [JsonProperty("lastName")]
        public LastName LastName { get; set; }
    }

}
