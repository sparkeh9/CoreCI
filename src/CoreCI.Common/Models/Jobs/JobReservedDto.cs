namespace CoreCI.Common.Models.Jobs
{
   using System;

   public class JobReservedDto
    {
        public Guid JobId { get; set; }
        public Guid BuildAgentToken { get; set; }
    }
}
