import {PLATFORM, autoinject, Container} from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { ValidationControllerFactory, ValidationController, validationMessages, ValidationRules, validateTrigger } from 'aurelia-validation';
import { ControllerValidateResult } from 'aurelia-validation';
import { SolutionService } from '../../../Services/SolutionService';
import { AddProjectDto } from '../../../Models/Dto/projects/AddProjectDto';
import { EventAggregator } from 'aurelia-event-aggregator';
import { GitViewModel } from './VcsForms/Git';
import { SvnViewModel } from './VcsForms/Svn';
import {BootstrapFormRenderer} from '../../../Infrastructure/BootstrapFormRenderer';

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
    private canSave:boolean;
    public vcsConfig: any;
    public selectedTab: any;
    public tabs: any= {
        git: { id: 'git', label: 'Git', viewModel: {} as GitViewModel },
        svn: { id: 'svn', label: 'SVN', viewModel: {} as SvnViewModel }
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
        this.vcsConfig = { };
        this.selectedTab = this.tabs.git;

        this.tabs.git.viewModel = container.get( GitViewModel );
        this.tabs.svn.viewModel = container.get( SvnViewModel );
        this.controller = controllerFactory.createForCurrentScope();
        this.controller.addRenderer(new BootstrapFormRenderer());
        this.controller.validateTrigger = validateTrigger.changeOrBlur;
        this.defineRules();
    }

    public async attached()
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

    
    public async validate()
    {
        await this.controller.validate();
        await this.selectedTab.viewModel.validate();
        console.log( 'validate');
    }

    public async addSolution()
    {
        const result: ControllerValidateResult = await this.controller.validate();
        const vcsResult: ControllerValidateResult = await this.selectedTab.viewModel.validate();

        if( !result.valid || !vcsResult.valid)
            return;

        console.log( 'valid' );
//        const solution: Solution = await this.solutionService.addSolution( this.model );
//        this.router.navigateToRoute( 'solution-projects', { id: solution.id }, { replace: true } );
    }

    private defineRules()
    {
        validationMessages[ 'IsRequired' ] = `\${$displayName} is required`;

        this.rules = ValidationRules
            .ensure( ( x: AddProjectDto ) => x.name )
            .required()
            .withMessageKey( 'IsRequired' )
            .ensure( ( x: AddProjectDto ) => x.solution )
            .required()
            .withMessageKey( 'IsRequired' )
            .rules;

        this.controller.addObject( this.model, this.rules );
    }
}
