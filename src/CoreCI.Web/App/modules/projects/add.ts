import { autoinject, PLATFORM } from 'aurelia-framework';
import { RouterConfiguration, Router } from 'aurelia-router';

@autoinject
export class AddProjectViewModel
{
    private readonly myTabs: any;
    public vcsConfig: any;
    public viewmodels: any = {
        git: PLATFORM.moduleName( 'modules/projects/VcsForms/Git' )
    };

    constructor()
    {
        this.myTabs = [
            { id: 'git', label: 'Git', active: true }
        ];
    }
}
