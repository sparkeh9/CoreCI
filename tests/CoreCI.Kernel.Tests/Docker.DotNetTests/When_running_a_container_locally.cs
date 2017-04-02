namespace CoreCI.Kernel.Tests.Docker.DotNetTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Docker.DotNet;
    using global::Docker.DotNet.Models;
    using global::Docker.DotNet.X509;
    using Kernel.Infrastructure.Extensions;
    using Shouldly;
    using Xunit;

    public class When_running_a_container_locally
    {
        [ Fact ]
        public async Task Should_run_container()
        {
            string containerId = string.Empty;

            var credentials = new CertificateCredentials( new X509Certificate2( @"C:\Users\spark\.docker\machine\certs\key.pfx" ) );
            var config = new DockerClientConfiguration( new Uri( "http://192.168.99.100:2376" ), credentials );
            var client = config.CreateClient();

            try
            {
                await client.Images.PullImageAsync( new ImagesPullParameters
                {
                    All = false,
                    Parent = "microsoft/dotnet",
                    Tag = "1.1.0-sdk-projectjson"
                }, null );

                var container = await client.Containers.CreateContainerAsync( new CreateContainerParameters( new Config
                {
                    Image = "microsoft/dotnet:1.1.0-sdk-projectjson",
                    AttachStdout = true,
                    AttachStderr = true,
                    Entrypoint = new List<string>
                    {
                        "/bin/bash",
                        "-c",
                        "echo test && sleep 60 && echo exiting"
                    }
                } ) );

                containerId = container.ID;
                await client.Containers.StartContainerAsync( container.ID.Substring( 0, 12 ), new ContainerStartParameters() );

                var listOfContainers = await client.Containers.ListContainersAsync( new ContainersListParameters() );

                listOfContainers.Any( x => x.ID == container.ID ).ShouldBeTrue();
            }
            finally
            {
                if ( !containerId.IsNullOrWhiteSpace() )
                {
                    await client.Containers.StopContainerAsync( containerId, new ContainerStopParameters(), CancellationToken.None );
                    await client.Containers.RemoveContainerAsync( containerId, new ContainerRemoveParameters() );
                }
            }
        }
    }
}