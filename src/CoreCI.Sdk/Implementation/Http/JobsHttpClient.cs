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

        public async Task<IEnumerable<JobDto>> ListAvailableJobsAsync( BuildEnvironment environment )
        {
            var endpointUrl = coreCiHttpClient.BaseApiUrl
                                              .AppendPathSegment( "jobs" )
                                              .SetQueryParam( "environment", environment );
            var response = await coreCiHttpClient.FetchPagedAsync<JobDto>( endpointUrl );

            return response;
        }

        public async Task<JobDto> ReserveFirstAvailableJobAsync( BuildEnvironment environment )
        {
            var endpointUrl = coreCiHttpClient.BaseApiUrl
                                              .AppendPathSegment( "jobs" )
                                              .SetQueryParam( "environment", environment );
            var firstPage = await endpointUrl.Authenticate( coreCiHttpClient.Authenticator )
                                             .GetJsonAsync<PagedResponse<JobDto>>();

            var firstJob = firstPage.Values.FirstOrDefault();

            if ( firstJob == null )
                return null;

            await ReserveJobAsync( firstJob.JobId );

            return firstJob;
        }

        public async Task<JobDto> GetJobDetailsAsync( BuildEnvironment environment )
        {
            var job = await coreCiHttpClient.BaseApiUrl
                                            .AppendPathSegment( "jobs" )
                                            .AppendPathSegment( "details" )
                                            .SetQueryParam( "environment", environment )
                                            .Authenticate( coreCiHttpClient.Authenticator )
                                            .GetJsonAsync<JobDto>();

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
