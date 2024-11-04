using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NhlTvFetcher.Models.Json.ScheduleBroadcast {
    public class AwayTeam {
        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("placeName")] public PlaceName PlaceName { get; set; }

        [JsonPropertyName("abbrev")] public string Abbrev { get; set; }

        [JsonPropertyName("logo")] public string Logo { get; set; }

        [JsonPropertyName("darkLogo")] public string DarkLogo { get; set; }

        [JsonPropertyName("awaySplitSquad")] public bool AwaySplitSquad { get; set; }

        [JsonPropertyName("score")] public int Score { get; set; }

        [JsonPropertyName("radioLink")] public string RadioLink { get; set; }

        [JsonPropertyName("odds")] public List<Odd> Odds { get; set; }
    }

    public class FirstInitial {
        [JsonPropertyName("default")] public string Default { get; set; }
    }

    public class Game {
        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("season")] public int Season { get; set; }

        [JsonPropertyName("gameType")] public int GameType { get; set; }

        [JsonPropertyName("venue")] public Venue Venue { get; set; }

        [JsonPropertyName("neutralSite")] public bool NeutralSite { get; set; }

        [JsonPropertyName("startTimeUTC")] public DateTime StartTimeUTC { get; set; }

        [JsonPropertyName("easternUTCOffset")] public string EasternUTCOffset { get; set; }

        [JsonPropertyName("venueUTCOffset")] public string VenueUTCOffset { get; set; }

        [JsonPropertyName("venueTimezone")] public string VenueTimezone { get; set; }

        [JsonPropertyName("gameState")] public string GameState { get; set; }

        [JsonPropertyName("gameScheduleState")] public string GameScheduleState { get; set; }

        [JsonPropertyName("tvBroadcasts")] public List<TvBroadcast> TvBroadcasts { get; set; }

        [JsonPropertyName("awayTeam")] public AwayTeam AwayTeam { get; set; }

        [JsonPropertyName("homeTeam")] public HomeTeam HomeTeam { get; set; }

        [JsonPropertyName("periodDescriptor")] public PeriodDescriptor PeriodDescriptor { get; set; }

        [JsonPropertyName("gameOutcome")] public GameOutcome GameOutcome { get; set; }

        [JsonPropertyName("winningGoalie")] public WinningGoalie WinningGoalie { get; set; }

        [JsonPropertyName("winningGoalScorer")] public WinningGoalScorer WinningGoalScorer { get; set; }

        [JsonPropertyName("threeMinRecap")] public string ThreeMinRecap { get; set; }

        [JsonPropertyName("gameCenterLink")] public string GameCenterLink { get; set; }

        [JsonPropertyName("threeMinRecapFr")] public string ThreeMinRecapFr { get; set; }

        [JsonPropertyName("ticketsLink")] public string TicketsLink { get; set; }
    }

    public class GameOutcome {
        [JsonPropertyName("lastPeriodType")] public string LastPeriodType { get; set; }
    }

    public class GameWeek {
        [JsonPropertyName("date")] public string Date { get; set; }

        [JsonPropertyName("dayAbbrev")] public string DayAbbrev { get; set; }

        [JsonPropertyName("numberOfGames")] public int NumberOfGames { get; set; }

        [JsonPropertyName("games")] public List<Game> Games { get; set; }
    }

    public class HomeTeam {
        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("placeName")] public PlaceName PlaceName { get; set; }

        [JsonPropertyName("abbrev")] public string Abbrev { get; set; }

        [JsonPropertyName("logo")] public string Logo { get; set; }

        [JsonPropertyName("darkLogo")] public string DarkLogo { get; set; }

        [JsonPropertyName("homeSplitSquad")] public bool HomeSplitSquad { get; set; }

        [JsonPropertyName("score")] public int Score { get; set; }

        [JsonPropertyName("radioLink")] public string RadioLink { get; set; }

        [JsonPropertyName("odds")] public List<Odd> Odds { get; set; }
    }

    public class LastName {
        [JsonPropertyName("default")] public string Default { get; set; }

        [JsonPropertyName("cs")] public string Cs { get; set; }

        [JsonPropertyName("sk")] public string Sk { get; set; }

        [JsonPropertyName("fi")] public string Fi { get; set; }
    }

    public class Odd {
        [JsonPropertyName("providerId")] public int ProviderId { get; set; }

        [JsonPropertyName("value")] public string Value { get; set; }
    }

    public class OddsPartner {
        [JsonPropertyName("partnerId")] public int PartnerId { get; set; }

        [JsonPropertyName("country")] public string Country { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("imageUrl")] public string ImageUrl { get; set; }

        [JsonPropertyName("siteUrl")] public string SiteUrl { get; set; }

        [JsonPropertyName("bgColor")] public string BgColor { get; set; }

        [JsonPropertyName("textColor")] public string TextColor { get; set; }

        [JsonPropertyName("accentColor")] public string AccentColor { get; set; }
    }

    public class PeriodDescriptor {
        [JsonPropertyName("number")] public int Number { get; set; }

        [JsonPropertyName("periodType")] public string PeriodType { get; set; }
    }

    public class PlaceName {
        [JsonPropertyName("default")] public string Default { get; set; }

        [JsonPropertyName("fr")] public string Fr { get; set; }
    }

    public class RootObject {
        [JsonPropertyName("nextStartDate")] public string NextStartDate { get; set; }

        [JsonPropertyName("previousStartDate")] public string PreviousStartDate { get; set; }

        [JsonPropertyName("gameWeek")] public List<GameWeek> GameWeek { get; set; }

        [JsonPropertyName("oddsPartners")] public List<OddsPartner> OddsPartners { get; set; }

        [JsonPropertyName("preSeasonStartDate")] public string PreSeasonStartDate { get; set; }

        [JsonPropertyName("regularSeasonStartDate")]
        public string RegularSeasonStartDate { get; set; }

        [JsonPropertyName("regularSeasonEndDate")] public string RegularSeasonEndDate { get; set; }

        [JsonPropertyName("playoffEndDate")] public string PlayoffEndDate { get; set; }

        [JsonPropertyName("numberOfGames")] public int NumberOfGames { get; set; }
    }

    public class TvBroadcast {
        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("market")] public string Market { get; set; }

        [JsonPropertyName("countryCode")] public string CountryCode { get; set; }

        [JsonPropertyName("network")] public string Network { get; set; }
    }

    public class Venue {
        [JsonPropertyName("default")] public string Default { get; set; }
    }

    public class WinningGoalie {
        [JsonPropertyName("playerId")] public int PlayerId { get; set; }

        [JsonPropertyName("firstInitial")] public FirstInitial FirstInitial { get; set; }

        [JsonPropertyName("lastName")] public LastName LastName { get; set; }
    }

    public class WinningGoalScorer {
        [JsonPropertyName("playerId")] public int PlayerId { get; set; }

        [JsonPropertyName("firstInitial")] public FirstInitial FirstInitial { get; set; }

        [JsonPropertyName("lastName")] public LastName LastName { get; set; }
    }
}