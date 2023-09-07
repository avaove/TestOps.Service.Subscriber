#nullable disable warnings


namespace TestOps.Subscribers.VideoRecording.Options
{
    /// <summary>
    /// Contains configuration for Grid.
    /// </summary>
    public class GridApiConfig
    {
        public const string CONFIG_SECTION_NAME = @"GridApiConfig";
        public string GridGraphQlEndpoint { get; init; }
    }
}
