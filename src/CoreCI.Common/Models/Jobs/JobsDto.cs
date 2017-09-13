namespace CoreCI.Common.Models.Jobs
{
   using System;
   using System.Collections.Generic;

   public class JobsDto
    {
        public BuildEnvironment Environment { get; set; }
        public Guid JobId { get; set; }
        public Dictionary<string, Link> Links { get; set; }
    }
}
