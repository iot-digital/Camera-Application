using ParkingDemo.DTOs;

namespace ParkingDemo.Services;

public class MessageQueueService
{
    private int _messageId;

    public Dictionary<int, MessageTrackItem> Queue { get; set; }

    public MessageQueueService()
    {
        Queue = new();
    }

    public MessageTrackItem? SetMessage(MessageTrackItem messageTrackItem)
    {
        if (messageTrackItem is null)
            return null;

        int id = GetNewMessageId();

        messageTrackItem.Message.Id = id;
        Queue.Add(id, messageTrackItem);

        return messageTrackItem;
    }

    public bool SetMessageDBRowId(int messageId, int dbRowId)
    {
        if (!Queue.ContainsKey(messageId))
        {
            return false;
        }

        var item = Queue[messageId];

        if (item is null)
            return false;

        item.DbRowId = dbRowId;
        Queue[messageId] = item;

        return true;
    }

    public MessageTrackItem? GetMessageById(int messageId)
    {
        return !Queue.ContainsKey(messageId) ? null : Queue[messageId];
    }

    public bool RemoveMessage(int messageId)
    {
        return Queue.Remove(messageId);
    }

    private int GetNewMessageId()
    {
        _messageId += 1;
        return _messageId;
    }
}