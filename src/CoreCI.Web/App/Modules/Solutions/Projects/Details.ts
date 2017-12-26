import { autoinject, PLATFORM } from 'aurelia-framework';
import {RouterConfiguration} from 'aurelia-router/dist/aurelia-router';
import {Router} from 'aurelia-router/dist/aurelia-router';

@autoinject
export class SolutionDetailsViewModel
{
    private router: Router;

    public configureRouter( config: RouterConfiguration, router: Router )
    {
        this.router = router;

        config.map( [
            {
                route: ['','list'],
                name: 'projects-add',
                moduleId: PLATFORM.moduleName( './List' ),
                title: 'List'
            }
        ] );
    }
}
