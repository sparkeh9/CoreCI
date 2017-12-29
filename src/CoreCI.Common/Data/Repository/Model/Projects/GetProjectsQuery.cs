namespace CoreCI.Common.Data.Repository.Model.Projects
{
    using System.Collections.Generic;

    public class GetProjectsQuery
    {
        public string Name { get; set; }
        public string Solution { get; set; }
        public IEnumerable<(string name, SortingDirection direction)> Sort { get; set; } = new List<(string name, SortingDirection direction)>();
        public int Page { get; set; } = 1;
    }


}
