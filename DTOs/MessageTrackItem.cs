namespace ParkingDemo.DTOs;

public class MessageTrackItem
{
    public Message Message { get; set; } = null!;
    public bool IsAcknowledged { get; set; }
    public DateTime? SentAt { get; set; }
    public int DeviceId { get; set; }
    public int DbRowId { get; set; }
    public string Value { get; set; } = null!;

    public MessageTrackItem()
    {
        Init();
    }

    public MessageTrackItem(Message message, int deviceId, string value)
    {
        Message = message;
        DeviceId = deviceId;
        Value = value;

        Init();
    }

    private void Init()
    {
        IsAcknowledged = false;
        SentAt = DateTime.UtcNow;
    }
}