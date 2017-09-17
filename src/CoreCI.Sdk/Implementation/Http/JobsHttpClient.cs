namespace CoreCI.Sdk.Implementation.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Models;
    using Common.Models.Jobs;
    using Flurl;
    using Flurl.Http;

    public class JobsHttpClient : IJobs
    {
        private readonly CoreCIHttpClient coreCiHttpClient;

        public JobsHttpClient( CoreCIHttpClient coreCiHttpClient )
        {
            this.coreCiHttpClient = coreCiHttpClient;
        }

        public async Task<IEnumerable<JobsDto>> ListAvailableJobsAsync( BuildEnvironment environment )
        {
            var endpointUrl = coreCiHttpClient.BaseApiUrl
                                              .AppendPathSegment( "jobs" )
                                              .SetQueryParam( "environment", environment );
            var response = await coreCiHttpClient.FetchPagedAsync<JobsDto>( endpointUrl );

            return response;
        }

        public async Task<JobsDto> ReserveFirstAvailableJobAsync( BuildEnvironment environment )
        {
            var endpointUrl = coreCiHttpClient.BaseApiUrl
                                              .AppendPathSegment( "jobs" )
                                              .SetQueryParam( "environment", environment );
            var firstPage = await endpointUrl.Authenticate( coreCiHttpClient.Authenticator )
                                             .GetJsonAsync<PagedResponse<JobsDto>>();

            var firstJob = firstPage.Values.FirstOrDefault();

            if ( firstJob == null )
                return null;

            await ReserveJobAsync( firstJob.JobId );

            return firstJob;
        }

        public async Task<JobsDto> GetJobDetailsAsync( BuildEnvironment environment )
        {
            var job = await coreCiHttpClient.BaseApiUrl
                                            .AppendPathSegment( "jobs" )
                                            .AppendPathSegment( "details" )
                                            .SetQueryParam( "environment", environment )
                                            .Authenticate( coreCiHttpClient.Authenticator )
                                            .GetJsonAsync<JobsDto>();

            return job;
        }

        public async Task<JobReservedDto> ReserveJobAsync( Guid guid )
        {
            var jobReservedDto = await coreCiHttpClient.BaseApiUrl
                                                       .AppendPathSegment( "jobs" )
                                                       .AppendPathSegment( guid )
                                                       .AppendPathSegment( "reserve" )
                                                       .Authenticate( coreCiHttpClient.Authenticator )
                                                       .PostJsonAsync( new { } )
                                                       .ReceiveJson<JobReservedDto>();

            return jobReservedDto;
        }
    }
}
