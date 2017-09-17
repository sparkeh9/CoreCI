namespace CoreCI.BuildAgent.Windows
{
    using System;
    using System.Timers;
    using Common;
    using Topshelf;
    using CoreCI.Common.Models;

    public class WindowsBuildAgentService : ServiceControl
    {
        private readonly IBuildAgentDaemon buildAgentDaemon;

        private Timer timer;

        public WindowsBuildAgentService( IBuildAgentDaemon buildAgentDaemon )
        {
            this.buildAgentDaemon = buildAgentDaemon;
            this.buildAgentDaemon.PollStatusChanged += OnBuildAgentDaemonPollStatusChanged;
        }

        public bool Start( HostControl hostControl )
        {
            timer = new Timer( 1 );
            timer.Elapsed += async ( sender, args ) =>
                             {
                                 timer.Interval = TimeSpan.FromSeconds( 30 ).TotalMilliseconds;
                                 await buildAgentDaemon.InvokeAsync( BuildEnvironment.Windows );
                             };
            timer.Start();
            return true;
        }

        public bool Stop( HostControl hostControl )
        {
            timer.Enabled = false;
            return true;
        }

        private void OnBuildAgentDaemonPollStatusChanged( object sender, bool enabled )
        {
            if ( timer != null )
            {
                timer.Enabled = enabled;
            }
        }
    }
}
