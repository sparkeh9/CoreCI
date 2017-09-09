namespace CoreCI.Web.Api.v1.Models
{
    using System;
    using System.Collections.Generic;
    using Api.Models;

    public class JobsDto
    {
        public BuildEnvironment Environment { get; set; }
        public Guid JobId { get; set; }
        public Dictionary<string, Link> Links { get; set; }
    }
}
