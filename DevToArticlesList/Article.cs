using System.Text.Json.Serialization;

namespace DevToArticlesList;

public record Article
{
    [JsonPropertyName("type_of")]
    public string TypeOf { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("published")]
    public bool Published { get; set; }

    [JsonPropertyName("published_at")]
    public DateTime PublishedAt { get; set; }

    [JsonPropertyName("slug")]
    public string Slug { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("comments_count")]
    public int CommentsCount { get; set; }

    [JsonPropertyName("public_reactions_count")]
    public int PublicReactionsCount { get; set; }

    [JsonPropertyName("page_views_count")]
    public int PageViewsCount { get; set; }

    [JsonPropertyName("published_timestamp")]
    public DateTime PublishedTimestamp { get; set; }

    [JsonPropertyName("body_markdown")]
    public string BodyMarkdown { get; set; }

    [JsonPropertyName("positive_reactions_count")]
    public int PositiveReactionsCount { get; set; }

    [JsonPropertyName("cover_image")]
    public string CoverImage { get; set; }

    [JsonPropertyName("tag_list")]
    public List<string> TagList { get; set; }

    [JsonPropertyName("canonical_url")]
    public string CanonicalUrl { get; set; }

    [JsonPropertyName("reading_time_minutes")]
    public int ReadingTimeMinutes { get; set; }

    [JsonPropertyName("user")]
    public User User { get; set; }
}

public record User
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("twitter_username")]
    public string TwitterUsername { get; set; }

    [JsonPropertyName("github_username")]
    public string GithubUsername { get; set; }

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("website_url")]
    public string WebsiteUrl { get; set; }

    [JsonPropertyName("profile_image")]
    public string ProfileImage { get; set; }

    [JsonPropertyName("profile_image_90")]
    public string ProfileImage90 { get; set; }
}