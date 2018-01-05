import { autoinject, bindable, bindingMode } from 'aurelia-framework';
import { ControllerValidateResult, ValidationControllerFactory, ValidationController, validationMessages, ValidationRules, validateTrigger } from 'aurelia-validation';
import { AddGitProjectDto } from '../../../../Models/Dto/projects/Vcs/AddGitProjectDto';
import {BootstrapFormRenderer} from '../../../../Infrastructure/BootstrapFormRenderer';
import {AddSvnProjectDto} from '../../../../Models/Dto/projects/Vcs/AddSvnProjectDto';

@autoinject
export class SvnViewModel
{
    private readonly controller: ValidationController;
    public rules: any;

    @bindable( { defaultBindingMode: bindingMode.twoWay } )
    public model: AddGitProjectDto;

    constructor( controllerFactory: ValidationControllerFactory )
    {
        this.model = {
            repositoryUrl: ''
        };
        this.controller = controllerFactory.createForCurrentScope();
        
        this.controller.addRenderer(new BootstrapFormRenderer());
        this.controller.validateTrigger = validateTrigger.changeOrBlur;
        this.defineRules();
    }

    public async validate(): Promise<ControllerValidateResult>
    {
        return await this.controller.validate();
    }

    private defineRules()
    {
        validationMessages[ 'IsRequired' ] = `\${$displayName} is required`;
        this.rules = ValidationRules
            .ensure( ( x: AddSvnProjectDto ) => x.repositoryUrl )
            .required()
            .withMessageKey( 'IsRequired' )
            .rules;

        this.controller.addObject( this.model, this.rules );
    }
}
