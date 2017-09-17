﻿namespace CoreCI.Sdk.Implementation.Http
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Models;
    using Common.Models.Jobs;
    using Flurl;
    using Flurl.Http;
    using MongoDB.Bson;

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

        public async Task<(JobDto job, JobReservedDto reservation)?> ReserveFirstAvailableJobAsync( BuildEnvironment environment )
        {
            var endpointUrl = coreCiHttpClient.BaseApiUrl
                                              .AppendPathSegment( "jobs" )
                                              .SetQueryParam( "environment", environment );
            var firstPage = await endpointUrl.Authenticate( coreCiHttpClient.Authenticator )
                                             .GetJsonAsync<PagedResponse<JobDto>>();

            var firstJob = firstPage.Values.FirstOrDefault();

            if ( firstJob == null )
                return null;

            var reservation = await ReserveJobAsync( firstJob.JobId );

            return (firstJob, reservation);
        }

        public async Task<JobDto> GetJobDetailsAsync( ObjectId jobId )
        {
            var job = await coreCiHttpClient.BaseApiUrl
                                            .AppendPathSegment( "jobs" )
                                            .AppendPathSegment( jobId )
                                            .Authenticate( coreCiHttpClient.Authenticator )
                                            .GetJsonAsync<JobDto>();

            return job;
        }

        public async Task<JobReservedDto> ReserveJobAsync( ObjectId jobId )
        {
            var jobReservedDto = await coreCiHttpClient.BaseApiUrl
                                                       .AppendPathSegment( "jobs" )
                                                       .AppendPathSegment( jobId )
                                                       .AppendPathSegment( "reserve" )
                                                       .Authenticate( coreCiHttpClient.Authenticator )
                                                       .PostJsonAsync( new { } )
                                                       .ReceiveJson<JobReservedDto>();

            return jobReservedDto;
        }
    }
}
