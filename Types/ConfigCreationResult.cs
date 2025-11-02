namespace SimpleLogManager.Types
{
    public record ConfigCreationResult<TOptions>(
        string exceptionMessage,
        TOptions? Options
    );
}