
namespace server
{
    public class ClientState
    {
        public enum State
        {
            ReadingProlog,
            ReadingHeaders,
            ReadingContent,
            WritingHeaders,
            WritingContent,
            Closed
        }
    }
}
