import { autoinject } from 'aurelia-framework';
import { HttpClientConfiguration, HttpClient, json } from 'aurelia-fetch-client';
import {ListSolutionsDto} from '../Models/Dto/solutions/ListSolutionsDto';
import {AddSolutionDto} from '../Models/Dto/solutions/AddSolutionDto';
import {Solution} from '../Models/Dto/solutions/Solution';
import {IPagedResult} from '../Models/Dto/IPagedResult';
import * as URI from 'urijs'
var config = require( '../Config/app.config.json' );

@autoinject
export class SolutionService
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

    public async listSolutions( filter: ListSolutionsDto ): Promise<IPagedResult<Solution>>
    {
        const uri = URI( config.api.solutions.list )
            .search(filter)
            .toString();

        return ( await this.httpClient.fetch( uri, {
            method: 'get'
        } ) ).json();
    }

    public async addSolution( project: AddSolutionDto ): Promise<Solution>
    {
        return ( await this.httpClient.fetch( config.api.solutions.add, {
            method: 'post',
            body: json( project )
        } ) ).json();
    }
}
