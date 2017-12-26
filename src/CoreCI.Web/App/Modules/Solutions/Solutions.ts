import { autoinject, PLATFORM } from 'aurelia-framework';
import { RouterConfiguration, Router } from 'aurelia-router';
@autoinject
export class Solutions
{
    private router: Router;

    public configureRouter( config: RouterConfiguration, router: Router )
    {
        this.router = router;

        config.map( [
            {
                route: ['','list'],
                name: 'solutions-add',
                moduleId: PLATFORM.moduleName( './List' ),
                title: 'List'
            },
            {
                route: 'add',
                name: 'solutions-add',
                moduleId: PLATFORM.moduleName( './Add' ),
                nav: true,
                title: 'Add'
            },
            {
                route: ':id',
                name: 'solution-projects',
                moduleId: PLATFORM.moduleName( './Projects/Details' ),
                nav: false,
                title: 'Details'
            },
        ] );
    }
}
