import { autoinject, PLATFORM } from 'aurelia-framework';
import { RouterConfiguration, Router } from 'aurelia-router';

@autoinject
export default class App
{
    private router: Router;

    
    public attached()
    {
        this.mapNavigation( this.router );
    }

    public configureRouter( config: RouterConfiguration, router: Router )
    {
        this.router = router;
        config.title = 'CoreCI';

        config.map( [
            {
                route: [ '', 'dashboard' ],
                name: 'dashboard',
                moduleId: PLATFORM.moduleName( './Modules/Dashboard/Dashboard' ),
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
                moduleId: PLATFORM.moduleName( './Modules/Projects/Projects' ),
                nav: true,
                title: 'Projects',
            },
            {
                route: 'projects/add',
                name: 'projects-add',
                moduleId: PLATFORM.moduleName( './Modules/Projects/Add' ),
                nav: true,
                title: 'Add Project',
                settings: {
                    parentRoute: 'projects'
                }
            }
        ] );
    }

    private mapNavigation( router )
    {
        console.log( 'Map children to navigation items.' );
        var menuItems: any[] = [];
        router.navigation.forEach( menuItem =>
        {
            if( menuItem.settings.parentRoute )
            {
                // Submenu children
                const parent = menuItems.find( x => x.relativeHref == menuItem.settings.parentRoute );
                // If it doesn't exist, then something went wrong, so not checking
                parent.children.push( menuItem );
            }
            else
            {
                // Just insert.  It should not be there multiple times or it's a bad route
                menuItems[ menuItem ] = menuItems[ menuItem ] || [];
                // Create empty children
                menuItem.children = [];
                menuItems.push( menuItem );
            }
        } );
        return menuItems;
    }
}
