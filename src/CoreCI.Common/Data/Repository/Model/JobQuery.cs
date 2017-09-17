namespace CoreCI.Common.Data.Repository.Model
{
    using System.Collections.Generic;
    using Models;

    public class JobQuery
    {
        public bool HasFilter => BuildEnvironment.HasValue;
        public BuildEnvironment? BuildEnvironment { get; set; } = null;
        public JobStatus? JobStatus { get; set; } = null;
        public IEnumerable<(string name, SortingDirection direction)> Sort { get; set; } = new List<(string name, SortingDirection direction)>();
        public int Page { get; set; } = 1;
    }
}
