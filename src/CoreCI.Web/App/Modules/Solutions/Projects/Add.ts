import {PLATFORM, autoinject, Container} from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { ValidationControllerFactory, ValidationController, validationMessages, ValidationRules, Validator } from 'aurelia-validation';
import { ControllerValidateResult } from 'aurelia-validation';
import { SolutionService } from '../../../Services/SolutionService';
import { AddProjectDto } from '../../../Models/Dto/projects/AddProjectDto';
import { EventAggregator } from 'aurelia-event-aggregator';
import {GitViewModel} from './VcsForms/Git';
import {SvnViewModel} from './VcsForms/Svn';

@autoinject
export class AddProjectViewModel
{
    private readonly controller: ValidationController;
    private readonly router: Router;
    private readonly solutionService: SolutionService;
    private readonly eventAggregator: EventAggregator;

    public rules: any;
    public model: AddProjectDto;
    
    private solutionId;
    public vcsConfig: any;
    public selectedTab: any;
    public tabs: any= {
        git: { id: 'git', label: 'Git' },
        svn: { id: 'svn', label: 'SVN' }
    };
    public viewmodels: any = {
        git: {},
        svn: {}
    };

    private subscriber;

    constructor( container: Container, controllerFactory: ValidationControllerFactory, projectService: SolutionService, router: Router, eventAggregator: EventAggregator )
    {
        this.router = router;
        this.solutionService = projectService;
        this.eventAggregator = eventAggregator;
        this.model = {
            name: '',
            solution: ''
        };
        this.vcsConfig = { abc: 123 };
        this.controller = controllerFactory.createForCurrentScope();
        this.defineRules();
        this.selectedTab = this.tabs.git;

        this.viewmodels.git = container.get( GitViewModel );
        this.viewmodels.svn = container.get( SvnViewModel );

        console.log( this.selectedTab );
    }

    public attached()
    {
        this.eventAggregator.subscribe( 'tabs:selected', response =>
        {
            this.selectedTab = response;
        } );
    }

    private async activate( params )
    {
        this.model.solution = params.id;
    }

    public async addSolution()
    {
        console.log( this.viewmodels.git );

        const result: ControllerValidateResult = await this.controller.validate();

        if( !result.valid )
        {
            return;
        }

        console.log( 'valid' );
//        const solution: Solution = await this.solutionService.addSolution( this.model );
//        this.router.navigateToRoute( 'solution-projects', { id: solution.id }, { replace: true } );
    }

    private defineRules()
    {
        validationMessages[ 'IsRequired' ] = `\${$displayName} is required`;

        ValidationRules
            .ensure( ( x: AddProjectDto ) => x.name )
            .required()
            .withMessageKey( 'IsRequired' )
            .ensure( ( x: AddProjectDto ) => x.solution )
            .required()
            .withMessageKey( 'IsRequired' )
            .on( this.model );

//        this.controller.addObject( this.model, this.rules );
    }
}
