import { autoinject, bindable, bindingMode } from 'aurelia-framework';
import { validationMessages, ValidationRules } from 'aurelia-validation';
import { AddGitProjectDto } from '../../../../Models/Dto/projects/Vcs/AddGitProjectDto';

@autoinject
export class GitViewModel
{
    @bindable( { defaultBindingMode: bindingMode.twoWay } )
    public model: AddGitProjectDto;

    constructor()
    {
        this.model = {
            repositoryUrl: ''
        };
        this.defineRules();
    }
//
//    public activate( activateModel )
//    {
//        this.model = activateModel;
//    }

    private defineRules()
    {
        validationMessages[ 'IsRequired' ] = `\${$displayName} is required`;
        ValidationRules
            .ensure( ( x: AddGitProjectDto ) => x.repositoryUrl )
            .required()
            .withMessageKey( 'IsRequired' )
            .on( this.model );
    }
}
