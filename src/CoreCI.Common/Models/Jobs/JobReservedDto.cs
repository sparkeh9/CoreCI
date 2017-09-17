namespace CoreCI.Common.Models.Jobs
{
   using System;
   using MongoDB.Bson;

    public class JobReservedDto
    {
        public ObjectId JobId { get; set; }
        public Guid BuildAgentToken { get; set; }
    }
}
