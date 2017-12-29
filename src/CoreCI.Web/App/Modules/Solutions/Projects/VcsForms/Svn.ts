import { autoinject, bindable } from 'aurelia-framework';
import { validationMessages, ValidationRules } from 'aurelia-validation';
import { AddGitProjectDto } from '../../../../Models/Dto/projects/Vcs/AddGitProjectDto';

@autoinject
export class SvnViewModel
{
    @bindable
    public model: any;

    public activate( activateModel )
    {
        validationMessages[ 'IsRequired' ] = `\${$displayName} is required`;
        this.model = activateModel;

        ValidationRules
            .ensure( ( x: AddGitProjectDto ) => x.repositoryUrl )
                .required()
                .withMessageKey( 'IsRequired' )
            .on( this.model );
    }
}
