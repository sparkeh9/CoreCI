using System;
using System.IO;
using CoreCI.Kernel.PipelineProcessor;
using Microsoft.Extensions.Options;

namespace CoreCI.Kernel.Tests.Infrastructure.Helpers
{
    public static class PipelineProcessorOptionsHelper
    {
        public static IOptions<PipelineProcessorOptions> GenerateOptionsForTest()
        {
            var endpoint = Environment.GetEnvironmentVariable("DOCKER_HOST");
            var certPath = Environment.GetEnvironmentVariable("DOCKER_CERT_PATHabc");
            return Options.Create(new PipelineProcessorOptions
            {
                RemoteEndpoint = endpoint,
                PfxPath = certPath,
                Workspace = Path.Combine(ResourceHelpers.GetTempFilePath(), "workspaces")
            });
        }
    }
}