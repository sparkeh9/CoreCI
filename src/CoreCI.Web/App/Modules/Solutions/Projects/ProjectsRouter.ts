import { autoinject, PLATFORM } from 'aurelia-framework';
import { RouterConfiguration } from 'aurelia-router/dist/aurelia-router';
import { Router } from 'aurelia-router/dist/aurelia-router';

@autoinject
export class ProjectDetailsViewModel
{
    private router: Router;

    public configureRouter( config: RouterConfiguration, router: Router )
    {
        this.router = router;

        config.map( [
            {
                route: ['','list'],
                name: 'projects-list',
                moduleId: PLATFORM.moduleName( './List' ),
                title: 'List',
                settings: {hideFromBreadcrumb:true}
            },
            {
                route: 'add',
                name: 'projects-add',
                moduleId: PLATFORM.moduleName( './Add' ),
                nav: true,
                title: 'Add'
            }
        ] );
    }
}
