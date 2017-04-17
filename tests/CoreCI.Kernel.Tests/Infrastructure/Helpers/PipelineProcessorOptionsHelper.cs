using System;
using System.IO;
using CoreCI.Kernel.PipelineProcessor;
using Microsoft.Extensions.Options;

namespace CoreCI.Kernel.Tests.Infrastructure.Helpers
{
    public static class PipelineProcessorOptionsHelper
    {
        private const string DockerWindowsNamedPipe = "npipe://./pipe/docker_engine";

        public static IOptions<PipelineProcessorOptions> GenerateOptionsForTest()
        {
            var endpoint = Environment.GetEnvironmentVariable("DOCKER_HOST") ?? DockerWindowsNamedPipe;
            var certPath = Environment.GetEnvironmentVariable("DOCKER_CERT_PATH");
            return Options.Create(new PipelineProcessorOptions
            {
                RemoteEndpoint = endpoint,
                PfxPath = certPath,
                Workspace = Path.Combine(ResourceHelpers.GetTempFilePath(), "workspaces")
            });
        }
    }
}