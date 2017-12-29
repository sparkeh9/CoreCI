namespace CoreCI.Common.Data.Repository.Model
{
    using System.Collections.Generic;

    public class GetSolutionsQuery
    {
        public string Name { get; set; }
        public IEnumerable<(string name, SortingDirection direction)> Sort { get; set; } = new List<(string name, SortingDirection direction)>();
        public int Page { get; set; } = 1;
    }
}
