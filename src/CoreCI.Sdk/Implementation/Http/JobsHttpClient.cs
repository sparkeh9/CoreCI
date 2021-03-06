﻿namespace CoreCI.Sdk.Implementation.Http
{
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

        public async Task<IEnumerable<JobDto>> ListAvailableJobsAsync( IEnumerable<BuildEnvironment> environments )
        {
            var endpointUrl = coreCiHttpClient.BaseApiUrl
                                              .AppendPathSegment( "jobs" )
                                              .SetQueryParams( GenerateEnvironments( environments ) );
            var response = await coreCiHttpClient.FetchPagedAsync<JobDto>( endpointUrl );

            return response;
        }

        public async Task<JobDto> FindByIdAsync( ObjectId jobId )
        {
            var response = await coreCiHttpClient.BaseApiUrl
                                                 .AppendPathSegment( "jobs" )
                                                 .AppendPathSegment( jobId )
                                                 .GetJsonAsync<JobDto>();

            return response;
        }

        public async Task<(JobDto job, JobReservedDto reservation)?> ReserveFirstAvailableJobAsync( IEnumerable<BuildEnvironment> environments )
        {
            var endpointUrl = coreCiHttpClient.BaseApiUrl
                                              .AppendPathSegment( "jobs" )
                                              .SetQueryParams( GenerateEnvironments( environments ) );
            var firstPage = await endpointUrl.Authenticate( coreCiHttpClient.Authenticator )
                                             .GetJsonAsync<PagedResponse<JobDto>>();

            var firstJob = firstPage.Values.FirstOrDefault();

            if ( firstJob == null )
            {
                return null;
            }

            var reservation = await ReserveJobAsync( firstJob.JobId );
            var hydratedFirstJob = await FindByIdAsync( firstJob.JobId );

            return (hydratedFirstJob, reservation);
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


        public async Task ReportAsync( JobProgressDto jobProgress )
        {
            var response = await coreCiHttpClient.BaseApiUrl
                                                 .AppendPathSegment( "jobs" )
                                                 .AppendPathSegment( "report" )
                                                 .Authenticate( coreCiHttpClient.Authenticator )
                                                 .PostJsonAsync( jobProgress );
            response.EnsureSuccessStatusCode();
        }

        private IEnumerable<string> GenerateEnvironments( IEnumerable<BuildEnvironment> environments )
        {
            var i = 0;
            foreach ( var environment in environments )
            {
                yield return $"Environments[{i}].{nameof(BuildEnvironment.BuildOs)}={environment.BuildOs}";
                yield return $"Environments[{i}].{nameof(BuildEnvironment.BuildMode)}={environment.BuildMode}";
                i++;
            }
        }
    }
}
