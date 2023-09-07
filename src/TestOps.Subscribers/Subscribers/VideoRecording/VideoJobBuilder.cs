using k8s.Models;
using TestOps.Subscribers.VideoRecording.Options;


namespace TestOps.Subscribers.VideoRecording
{
    public class VideoJobBuilder
    {
        private readonly V1Job job;
        private readonly VideoJobConfig config;
        
        public VideoJobBuilder(VideoJobConfig config)
        {
            this.config = config;
            job = GenerateJob();

            V1Job GenerateJob() => new()
            {
                Metadata = CreateMetadata(),
                Spec = CreateJobSpec()
            };

            V1ObjectMeta CreateMetadata() => new() { Name = config.JobBaseName };

            V1JobSpec CreateJobSpec() => new()
            {
                TtlSecondsAfterFinished = config.TtlSecondsAfterFinished,
                CompletionMode = config.CompletionMode,
                BackoffLimit = config.BackOffLimit,
                Template = new V1PodTemplateSpec() { Spec = CreatePodSpec() }
            };

            V1PodSpec CreatePodSpec() => new()
            {
                ActiveDeadlineSeconds = config.ActiveDeadlineSeconds,
                RestartPolicy = config.RestartPolicy,
                Containers = new List<V1Container> 
                {
                    new()
                    {
                        Name = config.ContainerName,
                        Image = config.ImageName,
                        ImagePullPolicy = config.ImagePullPolicy
                    }
                }
            };
        }
        
        public VideoJobBuilder WithEnvironmentVariable(string name, string value)
        {
            var container = job.Spec.Template.Spec.Containers[0];

            container.Env ??= new List<V1EnvVar>();
            container.Env.Add(new V1EnvVar { Name = name, Value = value });

            return this;
        }

        public VideoJobBuilder WithJobNameIdentifier(string identifier)
        {
            string identifierNoDashes = identifier.Replace("-", "");
            job.Metadata.Name = $"{config.JobBaseName}-{identifierNoDashes}";

            return this;
        }

        public V1Job Build() => job;
    }
}
