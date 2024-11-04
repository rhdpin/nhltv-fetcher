using System;

///
/// Auto-generated classes to represent JSON schedule data
///

namespace NhlTvFetcher.Models.Json.Schedule {
    public class Rootobject {
        public Datum[] data { get; set; }
        public Links links { get; set; }
        public Meta meta { get; set; }
    }

    public class Links {
        public string first { get; set; }
        public string last { get; set; }
        public object prev { get; set; }
        public string next { get; set; }
    }

    public class Meta {
        public int current_page { get; set; }
        public int? from { get; set; }
        public int last_page { get; set; }
        public Link[] links { get; set; }
        public string path { get; set; }
        public int per_page { get; set; }
        public int? to { get; set; }
        public int total { get; set; }
    }

    public class Link {
        public string url { get; set; }
        public string label { get; set; }
        public bool active { get; set; }
    }

    public class Datum {
        public int id { get; set; }
        public DateTime startTime { get; set; }
        public object endTime { get; set; }
        public string srMatchId { get; set; }
        public Category1 category1 { get; set; }
        public Category2 category2 { get; set; }
        public Category3 category3 { get; set; }
        public string title { get; set; }
        public Clientmetadata[] clientMetadata { get; set; }
        public Homecompetitor homeCompetitor { get; set; }
        public Awaycompetitor awayCompetitor { get; set; }
        public Content[] content { get; set; }
    }

    public class Category1 {
        public int id { get; set; }
        public string name { get; set; }
        public object logo { get; set; }
        public object[] images { get; set; }
    }

    public class Category2 {
        public int id { get; set; }
        public string name { get; set; }
        public object logo { get; set; }
        public object[] images { get; set; }
    }

    public class Category3 {
        public int id { get; set; }
        public string name { get; set; }
        public object logo { get; set; }
        public object[] images { get; set; }
    }

    public class Homecompetitor {
        public int id { get; set; }
        public string name { get; set; }
        public string shortName { get; set; }
        public object fullName { get; set; }
        public string externalId { get; set; }
        public int competitorCategoryId { get; set; }
        public string srCompetitorId { get; set; }
        public string logo { get; set; }
        public Colors colors { get; set; }
    }

    public class Colors {
        public string textColor { get; set; }
        public string primaryColor { get; set; }
        public string secondaryColor { get; set; }
    }

    public class Awaycompetitor {
        public int id { get; set; }
        public string name { get; set; }
        public string shortName { get; set; }
        public object fullName { get; set; }
        public string externalId { get; set; }
        public int competitorCategoryId { get; set; }
        public string srCompetitorId { get; set; }
        public string logo { get; set; }
        public Colors1 colors { get; set; }
    }

    public class Colors1 {
        public string textColor { get; set; }
        public string primaryColor { get; set; }
        public string secondaryColor { get; set; }
    }

    public class Clientmetadata {
        public int id { get; set; }
        public string name { get; set; }
        public object description { get; set; }
        public object image { get; set; }
        public int global_id { get; set; }
        public Type type { get; set; }
    }

    public class Type {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Content {
        public int id { get; set; }
        public string teaserType { get; set; }
        public string externalMatchId { get; set; }
        public string path { get; set; }
        public int playtime { get; set; }
        public object adBreaks { get; set; }
        public int skipSeconds { get; set; }
        public int skipSecondsEnd { get; set; }
        public DateTime startTime { get; set; }
        public object endTime { get; set; }
        public object additionalStartTime { get; set; }
        public object autoBroadcast { get; set; }
        public object scheduleEncoding { get; set; }
        public object autoOffline { get; set; }
        public bool enableRecording { get; set; }
        public bool enableVpnDetection { get; set; }
        public bool enableDrmProtection { get; set; }
        public int? originEntryPoint { get; set; }
        public int? originStreamType { get; set; }
        public int? encodingDataCenter { get; set; }
        public Georestriction[] geoRestrictions { get; set; }
        public Editorial editorial { get; set; }
        public Status status { get; set; }
        public Payment payment { get; set; }
        public Distributiontype distributionType { get; set; }
        public Devicecategory1[] deviceCategories { get; set; }
        public Contenttype contentType { get; set; }
        public Clientcontentmetadata[] clientContentMetadata { get; set; }
    }

    public class Editorial {
        public Image image { get; set; }
        public Image1[] images { get; set; }
        public object[] additionalImages { get; set; }
        public Translations translations { get; set; }
    }

    public class Image {
        public string path { get; set; }
        public string[] manipulations { get; set; }
    }

    public class Translations {
        public En en { get; set; }
    }

    public class En {
        public string title { get; set; }
        public string description { get; set; }
    }

    public class Image1 {
        public string path { get; set; }
        public string[] manipulations { get; set; }
    }

    public class Status {
        public int id { get; set; } // 3==Live, 4==Replay
        public string name { get; set; }
        public bool isLive { get; set; }
        public bool isDelivered { get; set; }
        public bool live2Vod { get; set; }
        public DateTime? live2Vod_offline_datetime { get; set; }
        public bool vodPremiere { get; set; }
        public object vodPremiere_startTime { get; set; }
    }

    public class Payment {
        public int id { get; set; }
        public string name { get; set; }
        public Entitlement[] entitlements { get; set; }
    }

    public class Entitlement {
        public string entitlement { get; set; }
        public string price { get; set; }
        public string type { get; set; }
        public bool active { get; set; }
        public object tag { get; set; }
    }

    public class Distributiontype {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Contenttype {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Georestriction {
        public string isolist { get; set; }
        public Devicecategory deviceCategory { get; set; }
    }

    public class Devicecategory {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Devicecategory1 {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Clientcontentmetadata {
        public int id { get; set; }
        public string name { get; set; }
        public object description { get; set; }
        public object image { get; set; }
        public int global_id { get; set; }
        public Type1 type { get; set; }
    }

    public class Type1 {
        public int id { get; set; }
        public string name { get; set; }
    }
}