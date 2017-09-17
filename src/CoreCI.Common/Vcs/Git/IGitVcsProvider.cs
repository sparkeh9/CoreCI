namespace CoreCI.Common.Vcs.Git
{
    public interface IGitVcsProvider
    {
        void Clone( string url, string path );
    }
}
