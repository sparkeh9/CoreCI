namespace CoreCI.Common.Models.Jobs
{
    using System;
    using System.Collections.Generic;
    using Converter;
    using Newtonsoft.Json;

    public class JobsDto
    {
        public BuildEnvironment Environment { get; set; }
        public Guid JobId { get; set; }
        public Dictionary<string, Link> Links { get; set; }

        [ JsonConverter( typeof( VcsJobConverter ) ) ]
        public IVcsJob Data { get; set; }
    }
}
