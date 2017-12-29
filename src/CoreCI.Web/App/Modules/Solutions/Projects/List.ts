import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { IPagedResult } from '../../../Models/Dto/IPagedResult';
import { Project } from '../../../Models/Dto/projects/Solution';
import { ProjectService } from '../../../Services/ProjectService';
import { ListProjectsDto } from '../../../Models/Dto/projects/ListProjectsDto';

@autoinject
export class ListSolutionProjectsViewModel
{
    private readonly router: Router;
    private readonly projectService: ProjectService;
    private searchDto: ListProjectsDto;
    private projects: IPagedResult<Project>;
    private solutionId: string;

    constructor( router: Router, projectService: ProjectService )
    {
        this.router = router;
        this.projectService = projectService;
        this.searchDto = {
            name: '',
            solution: '',
            page: 1
        };
    }

    private async activate( params )
    {
        this.solutionId = params.id;
        this.searchDto.solution = params.id;
        this.projects = await this.projectService.listProjects( this.searchDto );
    }

    public addProject(): void
    {
        this.router.navigateToRoute( 'projects-add', { id: 'abc123' } );
    }
}
