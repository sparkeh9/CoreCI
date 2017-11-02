import { autoinject, PLATFORM } from 'aurelia-framework';
import { RouterConfiguration, Router } from 'aurelia-router';

@autoinject
export default class App
{
    private router: Router;

    public configureRouter( config: RouterConfiguration, router: Router )
    {
        this.router = router;
        config.title = 'Checkout';

        config.map([
            {
                route: ['', 'dashboard'],
                name: 'dashboard',
                moduleId: PLATFORM.moduleName('modules/dashboard/dashboard'),
                title: 'Dashboard',
                nav: true,
                settings: {
                    icon: 'icon-speedometer',
                    isNew: false
                }
            },
            {
                route: 'projects',
                name: 'projects',
                moduleId: PLATFORM.moduleName('./modules/projects/projects'),
                nav: true,
                title: 'Projects',
            }]);
    }
}
