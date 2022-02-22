public class AIEvent
{
    public int EventId;

    public Agent Sender;

    public object Details;

    public AIEvent(int eventId, Agent sender, object details)
    {
        EventId = eventId;
        Sender = sender;
        Details = details;
    }
}