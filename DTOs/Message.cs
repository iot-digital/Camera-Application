using System.Text.Json.Serialization;

namespace ParkingDemo.DTOs;

public class Message
{
    private string? _content;

    [JsonPropertyName("msg_id")]
    public int Id { get; set; }

    [JsonPropertyName("network_add")]
    public int Network { get; set; }

    [JsonPropertyName("node_add")]
    public int Node { get; set; }

    [JsonPropertyName("msg_content")]
    public string? Content
    {
        get => _content;
        set
        {
            _content = value;
            if (_content is not null && _content.Contains(':'))
            {
                var split = _content.Split(':');
                ContentType = split[0];
                ContentValue = ParseEncodedMessages(split[1], ContentType);
            }
            else
            {
                ContentType = "x";
                ContentValue = string.Empty;
            }
        }
    }

    [JsonPropertyName("images")]
    public List<string>? Images { get; set; } = new List<string>();

    [JsonIgnore]
    public string? ContentType { get; private set; }

    [JsonIgnore]
    public string? ContentValue { get; private set; }

    private static string ParseEncodedMessages(string rawContent, string contentType)
    {
        return string.IsNullOrEmpty(rawContent)
            ? string.Empty
            : contentType != "m"
                ? rawContent
                : ParseMessage(rawContent);
    }

    private static string ParseMessage(string contentValue)
    {
        // contentValue = 1,4,15,0,16,0,0|2,1,6,20,0,0,0

        return contentValue;
    }
}