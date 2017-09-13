namespace CoreCI.BuildAgent.Windows
{
    using Common;
    using System;
    using Nito.AsyncEx;
    using Topshelf;

    public class WindowsBuildAgentService : ServiceControl
    {
        private readonly IBuildAgentDaemon buildAgentDaemon;

        public WindowsBuildAgentService( IBuildAgentDaemon buildAgentDaemon )
        {
            this.buildAgentDaemon = buildAgentDaemon;
        }

        public bool Start( HostControl hostControl )
        {
            bool result = AsyncContext.Run( buildAgentDaemon.StartAsync );
            return result;
        }

        public bool Stop( HostControl hostControl )
        {
            bool result = AsyncContext.Run( buildAgentDaemon.StartAsync );
            return true;
        }
    }
}
