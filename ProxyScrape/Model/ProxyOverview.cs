using System.Text.Json.Serialization;

namespace ProxyScrape.Model;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class Data
    {
        [JsonPropertyName("account_type")]
        public string AccountType { get; set; }

        [JsonPropertyName("bandwidth")]
        public int Bandwidth { get; set; }

        [JsonPropertyName("campaign_id")]
        public object CampaignId { get; set; }

        [JsonPropertyName("discount_applied")]
        public int DiscountApplied { get; set; }

        [JsonPropertyName("duration")]
        public string Duration { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("expiry_date")]
        public long ExpiryDate { get; set; }

        [JsonPropertyName("external_parent_id")]
        public int ExternalParentId { get; set; }

        [JsonPropertyName("external_sub_user_id")]
        public int ExternalSubUserId { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("time_created")]
        public int TimeCreated { get; set; }

        [JsonPropertyName("users")]
        public List<User> Users { get; set; }

        [JsonPropertyName("plans")]
        public List<Plan> Plans { get; set; }
    }

    public class Plan
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("max_bytes")]
        public long MaxBytes { get; set; }

        [JsonPropertyName("bytes_used")]
        public long BytesUsed { get; set; }

        [JsonPropertyName("max_threads")]
        public int MaxThreads { get; set; }

        [JsonPropertyName("max_throughput")]
        public int MaxThroughput { get; set; }

        [JsonPropertyName("start_date")]
        public string StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string EndDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("bandwidth_gb")]
        public int BandwidthGb { get; set; }
    }

    public class RiptideApiResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("parent_id")]
        public int ParentId { get; set; }

        [JsonPropertyName("max_bytes")]
        public long MaxBytes { get; set; }

        [JsonPropertyName("used_bytes")]
        public long UsedBytes { get; set; }

        [JsonPropertyName("expiry_time")]
        public long ExpiryTime { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
    }

    public class ProxyOverview
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("data")]
        public Data Data { get; set; }

        [JsonPropertyName("account_type")]
        public string AccountType { get; set; }

        [JsonPropertyName("riptide_api_response")]
        public List<RiptideApiResponse> RiptideApiResponse { get; set; }
    }

    public class User
    {
        [JsonPropertyName("pp_api_login")]
        public string PpApiLogin { get; set; }

        [JsonPropertyName("pp_api_password")]
        public string PpApiPassword { get; set; }

        [JsonPropertyName("proxy_ip")]
        public string ProxyIp { get; set; }

        [JsonPropertyName("proxy_port")]
        public string ProxyPort { get; set; }
    }

