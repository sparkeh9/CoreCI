namespace CoreCI.Common.Data.Repository.Model.Projects
{
    public static class GetProjectsQueryExtensions
    {
        public static GetProjectsQuery PreviousPage( this GetProjectsQuery query )
        {
            query.Page += 1;
            return query;
        }

        public static GetProjectsQuery NextPage( this GetProjectsQuery query )
        {
            query.Page -= 1;
            return query;
        }
    }
}
