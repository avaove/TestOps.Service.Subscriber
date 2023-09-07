#nullable disable warnings


namespace TestOps.Subscribers.VideoRecording.Options
{
    /// <summary>
    /// Contains configuration for a video recording Kubernetes job.
    /// </summary>
    public class VideoJobConfig
    {
        public const string CONFIG_SECTION_NAME = @"VideoJobConfig";
        public string JobBaseName { get; init; }
        public string ContainerName { get; init; }
        public string ImageName { get; init; }
        public string ImagePullPolicy { get; init; }
        public int TtlSecondsAfterFinished { get; init; }
        public string CompletionMode { get; init; }
        public int ActiveDeadlineSeconds { get; init; }
        public string RestartPolicy { get; init; }
        public int BackOffLimit { get; init; }
        public Dictionary<string, EnvironmentVariable> JobEnvironmentVariables { get; init; }
        public string Namespace { get; init; }
    }

    /// <summary>
    /// Contains name and value for the environment variables passed to a video recording job.
    /// </summary>
    public class EnvironmentVariable
    {
        public string Name { get; init; }
        public string Value { get; init; }
    }
}