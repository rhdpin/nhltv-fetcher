using System;

///
/// Auto-generated classes to represent player settings related JSON response
///

namespace NhlTvFetcher.Models.Json.Player {
    public class Rootobject {
        public int videoid { get; set; }
        public int partnerid { get; set; }
        public object v5ident { get; set; }
        public string portalid { get; set; }
        public bool enablePip { get; set; }
        public bool enableOTTStats { get; set; }
        public bool enableOTTTwitter { get; set; }
        public bool useHlsDefaults { get; set; }
        public bool useDashDefaults { get; set; }
        public bool enableCountdown { get; set; }
        public bool isLivestream { get; set; }
        public bool ignoreAspectratio { get; set; }
        public bool downloadWhilePaused { get; set; }
        public bool isLive { get; set; }
        public bool isDelivered { get; set; }
        public string distributionType { get; set; }
        public string pathToIsLive { get; set; }
        public string image { get; set; }
        public DateTime date { get; set; }
        public bool allowMobileLive { get; set; }
        public bool allowDVR { get; set; }
        public string streamAccess { get; set; }
        public Streamurlproviderinfo streamUrlProviderInfo { get; set; }
        public string language { get; set; }
        public bool autoplay { get; set; }
        public bool enableProgressBar { get; set; }
        public bool enableTime { get; set; }
        public bool enableSeekForward { get; set; }
        public bool enableSeekBehind { get; set; }
        public int desiredStartTime { get; set; }
        public int desiredEndTime { get; set; }
        public int seekButtonSeconds { get; set; }
        public string backgroundImage { get; set; }
        public Trackprogress trackProgress { get; set; }
        public string title { get; set; }
        public Casting casting { get; set; }
        public bool allowFullScreen { get; set; }
        public bool fakeFullScreen { get; set; }
        public bool titleEnabled { get; set; }
        public bool posterTitleEnabled { get; set; }
        public bool loop { get; set; }
        public string group { get; set; }
        public string flashplayer { get; set; }
        public string aspectratio { get; set; }
        public bool useSuggestedPresentationDelay { get; set; }
        public bool edgeServerIpDetection { get; set; }
        public bool hardwareAcceleration { get; set; }
        public bool trackLiveEdge { get; set; }
        public Metainformation metaInformation { get; set; }
        public Advertisement advertisement { get; set; }
        public Quality quality { get; set; }
        public Share share { get; set; }
        public Skin skin { get; set; }
        public Customusermessages customUserMessages { get; set; }
        public Watermark watermark { get; set; }
        public Heroverlay herOverlay { get; set; }
        public bool enableWatchTogether { get; set; }
        public Multiplelanguage[] multipleLanguages { get; set; }
        public Timeline timeline { get; set; }
        public Analytic[] analytics { get; set; }
        public bool enableAutomaticCC { get; set; }
        public Endscreen endscreen { get; set; }
        public string description { get; set; }
    }

    public class Streamurlproviderinfo {
        public string providerClass { get; set; }
        public Data data { get; set; }
    }

    public class Data {
        public string streamAccessUrl { get; set; }
    }

    public class Trackprogress {
        public bool enabled { get; set; }
    }

    public class Casting {
        public Chromecast chromecast { get; set; }
        public bool airplay { get; set; }
    }

    public class Chromecast {
        public bool enabled { get; set; }
        public bool forceMediaUrl { get; set; }
        public string receiverId { get; set; }
        public string streamAccess { get; set; }
        public string credentialsUrl { get; set; }
        public bool useCallback { get; set; }
    }

    public class Metainformation {
        public string sports { get; set; }
        public object liga { get; set; }
        public string competition { get; set; }
    }

    public class Advertisement {
        public bool enabled { get; set; }
        public string client { get; set; }
        public bool skipEnabled { get; set; }
        public object skipTime { get; set; }
        public int pauseEnabled { get; set; }
        public Preroll[] prerolls { get; set; }
        public object[] invideos { get; set; }
        public Postroll[] postrolls { get; set; }
        public bool useNativeControls { get; set; }
    }

    public class Preroll {
        public string id { get; set; }
        public string url { get; set; }
    }

    public class Postroll {
        public string id { get; set; }
        public string url { get; set; }
    }

    public class Quality {
        public bool enabled { get; set; }
        public int startIndex { get; set; }
        public Label[] labels { get; set; }
    }

    public class Label {
        public string label { get; set; }
        public int bitrate { get; set; }
        public string resolution { get; set; }
    }

    public class Share {
        public bool enabled { get; set; }
        public string url { get; set; }
        public string[] pages { get; set; }
    }

    public class Skin {
        public int theme { get; set; }
        public Colors colors { get; set; }
    }

    public class Colors {
        public string background { get; set; }
        public string iconbuttonprimary { get; set; }
        public string iconbuttonhover { get; set; }
        public string label { get; set; }
        public string labelactive { get; set; }
        public string togglebuttonprimary { get; set; }
        public string togglebuttonactive { get; set; }
        public string togglebuttontext { get; set; }
        public string togglebuttontextactive { get; set; }
        public string slider { get; set; }
        public string sliderseekbarbuffer { get; set; }
        public string sliderbackground { get; set; }
    }

    public class Customusermessages {
        public string adCountdownText { get; set; }
        public string back { get; set; }
        public string cancel { get; set; }
        public string ccOverlay { get; set; }
        public string day { get; set; }
        public string days { get; set; }
        public string downArrow { get; set; }
        public string engagement { get; set; }
        public string error001 { get; set; }
        public string error100 { get; set; }
        public string error200 { get; set; }
        public string error201 { get; set; }
        public string error202 { get; set; }
        public string error203 { get; set; }
        public string error204 { get; set; }
        public string error205 { get; set; }
        public string error206 { get; set; }
        public string error207 { get; set; }
        public string error208 { get; set; }
        public string error209 { get; set; }
        public string error210 { get; set; }
        public string error300 { get; set; }
        public string error400 { get; set; }
        public string error500 { get; set; }
        public string error501 { get; set; }
        public string error502 { get; set; }
        public string error503 { get; set; }
        public string error600 { get; set; }
        public string error700 { get; set; }
        public string error900 { get; set; }
        public string error901 { get; set; }
        public string error902 { get; set; }
        public string error903 { get; set; }
        public string error904 { get; set; }
        public string error905 { get; set; }
        public string fastforward { get; set; }
        public string fullscreenback { get; set; }
        public string fullscreennormal { get; set; }
        public string general { get; set; }
        public string home { get; set; }
        public string hour { get; set; }
        public string hours { get; set; }
        public string minute { get; set; }
        public string minutes { get; set; }
        public string multiAudio { get; set; }
        public string nextVideo { get; set; }
        public string ok { get; set; }
        public string pause { get; set; }
        public string pauseAd { get; set; }
        public string pip { get; set; }
        public string play { get; set; }
        public string playAd { get; set; }
        public string recommended { get; set; }
        public string replay { get; set; }
        public string rewind { get; set; }
        public string second { get; set; }
        public string seconds { get; set; }
        public string settings { get; set; }
        public string settingsOverlay { get; set; }
        public string share { get; set; }
        public string share_this_label { get; set; }
        public string skipAd { get; set; }
        public string spoilerAlert { get; set; }
        public string startFromBeginning { get; set; }
        public string startFromLastSeen { get; set; }
        public string startFromLive { get; set; }
        public string streamEnded { get; set; }
        public string streamSoon { get; set; }
        public string tooltips { get; set; }
        public string trackProgress { get; set; }
        public string volume { get; set; }
        public string volumemute { get; set; }
    }

    public class Watermark {
        public bool enabled { get; set; }
        public string url { get; set; }
        public string position { get; set; }
    }

    public class Heroverlay {
        public bool enabled { get; set; }
        public object statId { get; set; }
        public int delay { get; set; }
    }

    public class Timeline {
        public string url { get; set; }
        public bool skipSpoilerAlert { get; set; }
        public bool ignoreTimelineText { get; set; }
        public int interval { get; set; }
    }

    public class Endscreen {
        public bool enabled { get; set; }
        public bool countdown { get; set; }
        public int countdownTime { get; set; }
        public int template { get; set; }
        public Related[] related { get; set; }
    }

    public class Related {
        public int id { get; set; }
        public string title { get; set; }
        public DateTime startDate { get; set; }
        public string image { get; set; }
        public string target { get; set; }
    }

    public class Multiplelanguage {
        public int id { get; set; }
        public string label { get; set; }
        public bool active { get; set; }
    }

    public class Analytic {
        public string handler { get; set; }
        public string url { get; set; }
        public string key { get; set; }
        public string title { get; set; }
        public string customData1 { get; set; }
        public string customData2 { get; set; }
        public string customData3 { get; set; }
        public string customData4 { get; set; }
        public string customData5 { get; set; }
        public string cdnProvider { get; set; }
        public string player { get; set; }
        public string videoId { get; set; }
        public bool isLive { get; set; }
        public object userId { get; set; }
    }
}