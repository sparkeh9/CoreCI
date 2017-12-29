import { autoinject } from 'aurelia-framework';
import { HttpClientConfiguration, HttpClient, json } from 'aurelia-fetch-client';
import { IPagedResult } from '../Models/Dto/IPagedResult';
import { ListProjectsDto } from '../Models/Dto/projects/ListProjectsDto';
import { Project } from '../Models/Dto/projects/Solution';
import { AddProjectDto } from '../Models/Dto/projects/AddProjectDto';
import * as URI from 'urijs'
var config = require( '../Config/app.config.json' );

@autoinject
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

    public async listProjects( filter: ListProjectsDto ): Promise<IPagedResult<Project>>
    {
        const uri = URI( config.api.projects.list )
            .search(filter)
            .toString();

        return ( await this.httpClient.fetch( uri, {
            method: 'get'
        } ) ).json();
    }

    public async addProject( project: AddProjectDto ): Promise<Project>
    {
        return ( await this.httpClient.fetch( config.api.projects.add, {
            method: 'post',
            body: json( project )
        } ) ).json();
    }
}
