import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';
import { ValidationControllerFactory, ValidationController, validationMessages, ValidationRules, Validator } from 'aurelia-validation';
import { SolutionService } from '../../Services/SolutionService';
import { ControllerValidateResult } from 'aurelia-validation';
import { AddSolutionDto } from '../../Models/Dto/solutions/AddSolutionDto';
import {Solution} from '../../Models/Dto/solutions/Solution';

@autoinject
export class AddSolutionViewModel
{
    private readonly controller: ValidationController;
    private readonly router: Router;
    private readonly solutionService: SolutionService;

    public rules: any;
    public model: AddSolutionDto;

    constructor( controllerFactory: ValidationControllerFactory, projectService: SolutionService, router: Router)
    {
        this.router = router;
        this.solutionService = projectService;
        this.model = {
            name:''
        };
        this.controller = controllerFactory.createForCurrentScope();
        this.defineRules();
    }

    public async addSolution()
    {
        const result: ControllerValidateResult = await this.controller.validate();

        if( !result.valid )
        {
            return;
        }

        let solution:Solution = await this.solutionService.addSolution( this.model );
//        this.router.navigate( 'add' );
        this.router.navigateToRoute('solutions-add', { id: solution.id }, { replace: true });
    }

    private defineRules()
    {
        validationMessages[ 'IsRequired' ] = `\${$displayName} is required`;

        this.rules = ValidationRules
            .ensure( ( x: AddSolutionDto ) => x.name )
            .required()
            .withMessageKey( 'IsRequired' )
            .rules;

        this.controller.addObject( this.model, this.rules );
    }

    //    private readonly myTabs: any;

    //    public vcsConfig: any;

    //    public viewmodels: any = {

    //        git: PLATFORM.moduleName( 'modules/projects/VcsForms/Git' )

    //    };

//

//    constructor()
//    {
//        this.myTabs = [
//            { id: 'git', label: 'Git', active: true }
//        ];
//    }
}
