namespace CoreCI.Sdk.Implementation.Http
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common;
    using Flurl;
    using Flurl.Http;
    using Common.Extensions;
    using Common.Models;

    internal static class CoreCIHttpClientExtensions
    {
        public static IFlurlClient Authenticate( this Url operand, IAuthenticator authenticator )
        {
            if ( authenticator == null )
            {
                return operand.WithHeader( "X-No-Auth", "1" );
            }

            return authenticator.Authenticate( operand );
        }

        public static async Task<IEnumerable<T>> FetchPagedAsync<T>( this ICoreCI client, Url url, int limit = int.MaxValue )
        {
            var response = await url.Authenticate( client.Authenticator )
                                    .GetJsonAsync<PagedResponse<T>>();

            var collection = new List<T>();
            collection.AddRange( response.Values );


            if ( collection.Count <= limit )
            {
                if ( !response.Next.IsNullOrWhiteSpace() )
                {
                    collection.AddRange( await client.FetchPagedAsync<T>( new Url( response.Next ) ) );
                }
            }

            return collection;
        }
    }
}
