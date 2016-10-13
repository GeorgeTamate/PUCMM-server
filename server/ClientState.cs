
namespace server
{
    public enum HttpClientState {
        ReadingProlog,
        ReadingHeaders,
        ReadingContent,
        WritingHeaders,
        WritingContent,
        Closed }
}
