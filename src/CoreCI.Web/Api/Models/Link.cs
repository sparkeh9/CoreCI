﻿namespace CoreCI.Web.Api.Models
{
    public class Link
    {
        public string Href { get; }

        public Link( string href )
        {
            Href = href;
        }
    }
}
