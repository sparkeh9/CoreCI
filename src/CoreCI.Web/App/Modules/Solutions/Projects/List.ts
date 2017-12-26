import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import {SolutionService} from '../../../Services/SolutionService';
import {ListSolutionsDto} from '../../../Models/Dto/solutions/ListSolutionsDto';
import {Solution} from '../../../Models/Dto/solutions/Solution';
import {IPagedResult} from '../../../Models/Dto/solutions/IPagedResult';

@autoinject
export class List
{
//    private readonly router: Router;
//    private readonly solutionService: SolutionService;
//    private searchDto: ListSolutionsDto;
//
//    private solutions: IPagedResult<Solution>;
//
//    constructor( router: Router, solutionService: SolutionService )
//    {
//        this.router = router;
//        this.solutionService = solutionService;
//        this.searchDto = {
//            name: '',
//            page: 1
//        };
//    }
//
//    private async activate(params)
//    {
//        this.solutions = await this.solutionService.listSolutions( this.searchDto );
//    }
//
//    public addSolution(): void
//    {
//        this.router.navigate( 'add' );
//    }
}
