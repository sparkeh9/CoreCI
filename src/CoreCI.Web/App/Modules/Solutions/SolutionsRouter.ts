import { autoinject, PLATFORM } from 'aurelia-framework';
import { RouterConfiguration, Router } from 'aurelia-router';
import { EventAggregator } from 'aurelia-event-aggregator';
import {INavigationEntry} from '../../Models/INavigationEntry';
import {NavigationViewModel} from '../../Models/NavigationViewModel';

@autoinject
export class Solutions
{
    private readonly eventAggregator: EventAggregator;
    private router: Router;

    constructor( eventAggregator: EventAggregator )
    {
        this.eventAggregator = eventAggregator;
    }
//
//    public attached()
//    {
//        this.eventAggregator.publish( 'CoreCI:NavigationUpdated', this.generateNav() );
//    }
//
//    public generateNav(): NavigationViewModel
//    {
//        const navigation = new NavigationViewModel();
//        navigation.name = 'Solutions';
//        navigation.navigationEntries = [
//            {
//                active: true,
//                text: 'List Solutions',
//                url: this.router.generate( 'solutions-list' ),
//                settings: { icon: 'icon-speedometer' }
//            }
//        ] as INavigationEntry[];
//
//        return navigation;
//    }

    public configureRouter( config: RouterConfiguration, router: Router )
    {
        this.router = router;

        config.map( [
            {
                route: ['','list'],
                name: 'solutions-list',
                moduleId: PLATFORM.moduleName( './List' ),
                title: 'List',
                settings: {hideFromBreadcrumb:true}
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
                moduleId: PLATFORM.moduleName( './Projects/ProjectsRouter' ),
                nav: false,
                title: 'Project List'
            }
        ] );
    }
}
