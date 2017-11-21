namespace Tars.Csharp.Net.Client.Tickets
{
    public interface ITicketListener
    {
        void OnResponseExpired<T>(Ticket<T> ticket);

        void OnResponseReceived<T>(Ticket<T> ticket);
    }
}