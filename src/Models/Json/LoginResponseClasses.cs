///
/// Auto-generated classes to represent login JSON response
///

namespace NhlTvFetcher.Models.Json.Login
{
    public class Rootobject
    {
        public Cleengprofile cleengProfile { get; set; }
        public Nhlprofile nhlProfile { get; set; }
        public string token { get; set; }
    }

    public class Cleengprofile
    {
        public int id { get; set; }
        public string externalId { get; set; }
        public Subscription[] subscriptions { get; set; }
        public Offer[] offers { get; set; }
        public Transaction[] transactions { get; set; }
    }

    public class Subscription
    {
        public int subscriptionId { get; set; }
        public string offerId { get; set; }
        public string status { get; set; }
        public int startedAt { get; set; }
        public int expiresAt { get; set; }
        public float nextPaymentPrice { get; set; }
        public string nextPaymentCurrency { get; set; }
        public int nextPaymentAt { get; set; }
        public string paymentGateway { get; set; }
        public string paymentMethod { get; set; }
        public string externalPaymentId { get; set; }
        public string offerTitle { get; set; }
        public string period { get; set; }
        public float totalPrice { get; set; }
        public bool inTrial { get; set; }
    }

    public class Offer
    {
        public string offerId { get; set; }
        public string status { get; set; }
        public int subscriptionId { get; set; }
        public int startedAt { get; set; }
        public int expiresAt { get; set; }
        public float nextPaymentPrice { get; set; }
        public string nextPaymentCurrency { get; set; }
        public int nextPaymentAt { get; set; }
        public string paymentGateway { get; set; }
        public string paymentMethod { get; set; }
        public string externalPaymentId { get; set; }
        public string offerTitle { get; set; }
        public string offerType { get; set; }
        public string period { get; set; }
        public float totalPrice { get; set; }
        public bool inTrial { get; set; }
        public object pendingSwitchId { get; set; }
    }

    public class Transaction
    {
        public string transactionId { get; set; }
        public int transactionDate { get; set; }
        public string offerId { get; set; }
        public string offerType { get; set; }
        public string offerTitle { get; set; }
        public string offerPeriod { get; set; }
        public string publisherSiteName { get; set; }
        public string transactionPriceExclTax { get; set; }
        public string transactionCurrency { get; set; }
        public string contentExternalId { get; set; }
        public string contentType { get; set; }
        public string shortUrl { get; set; }
        public string campaignId { get; set; }
        public string campaignName { get; set; }
        public object couponCode { get; set; }
        public string discountType { get; set; }
        public string discountRate { get; set; }
        public string discountValue { get; set; }
        public string discountedOfferPrice { get; set; }
        public string offerCurrency { get; set; }
        public string offerPriceExclTax { get; set; }
        public string applicableTax { get; set; }
        public string transactionPriceInclTax { get; set; }
        public string appliedExchangeRateCustomer { get; set; }
        public string customerId { get; set; }
        public string customerEmail { get; set; }
        public string customerLocale { get; set; }
        public string customerCountry { get; set; }
        public string customerIpCountry { get; set; }
        public string customerCurrency { get; set; }
        public string paymentMethod { get; set; }
        public string referalUrl { get; set; }
        public string transactionExternalData { get; set; }
        public string publisherId { get; set; }
        public string taxRate { get; set; }
    }

    public class Nhlprofile
    {
        public object address1 { get; set; }
        public object address2 { get; set; }
        public object city { get; set; }
        public string countryCode { get; set; }
        public object dayOfBirth { get; set; }
        public string email { get; set; }
        public bool emailVerified { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public object monthOfBirth { get; set; }
        public object phone { get; set; }
        public string postalCode { get; set; }
        public object stateCode { get; set; }
        public string username { get; set; }
        public Favoriteteam[] favoriteTeams { get; set; }
        public int id { get; set; }
        public object photoUrl { get; set; }
    }

    public class Favoriteteam
    {
        public bool favorite { get; set; }
        public int sequence { get; set; }
        public Team team { get; set; }
    }

    public class Team
    {
        public int id { get; set; }
        public string commonName { get; set; }
        public string triCode { get; set; }
        public int competitorId { get; set; }
        public int srteamid { get; set; }
    }
}
