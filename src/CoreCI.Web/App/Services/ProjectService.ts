import { autoinject } from 'aurelia-framework';
import { HttpClientConfiguration, HttpClient, json } from 'aurelia-fetch-client';
import config from 'config/app.config.json!json';

export class ProjectService
{
    private readonly httpClient: HttpClient;

    constructor( httpClient: HttpClient )
    {
        httpClient.configure( ( httpConfig: HttpClientConfiguration ) =>
        {
            httpConfig
                .useStandardConfiguration()
                .withBaseUrl( config.api.base )
                .withDefaults( {
                    credentials: 'include',
                    headers: {
                        'content-type': 'application/json',
                        'Accept': 'application/json'
                    }
                } );
        } );

        this.httpClient = httpClient;
    }

    public async addProject()
    {
        await this.httpClient.fetch( config.api.projects.add, {
            method: 'post',
            body: json( {} )
        } );
    }
}
