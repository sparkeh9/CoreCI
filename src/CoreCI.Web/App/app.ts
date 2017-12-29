import { autoinject, PLATFORM, observable } from 'aurelia-framework';
import { RouterConfiguration, Router } from 'aurelia-router';
import { EventAggregator } from 'aurelia-event-aggregator';
import { INavigationEntry } from './Models/INavigationEntry';
import { NavigationViewModel } from './Models/NavigationViewModel';

@autoinject
export default class App
{
    private readonly eventAggregator: EventAggregator;
    private router: Router;
    
    @observable
    public navigation: NavigationViewModel;
    private subscriber;

    constructor( aggregator: EventAggregator )
    {
        this.eventAggregator = aggregator;
    }

    public attached()
    {
        this.subscriber = this.eventAggregator.subscribe( 'CoreCI:NavigationUpdated', response =>
        {
            if( response == null || response.length === 0 )
            {
                this.navigation = this.generateNav();
            }
            else
            {
                this.navigation = response;
            }
        } );


        this.navigation = this.generateNav();
    }

    public generateNav(): NavigationViewModel
    {
        const navigation = new NavigationViewModel();
        navigation.name = 'Dashboard';
        navigation.navigationEntries = [
            {
                active: false,
                text: 'Solutions',
                url: this.router.generate( 'solutions' ),
                settings: { icon: 'icon-layers' }
            }
        ] as INavigationEntry[];
        
        return navigation;
    }

    public configureRouter( config: RouterConfiguration, router: Router )
    {
        this.router = router;
        config.title = 'CoreCI';
        config.options.pushState = true;
        config.map( [
            {
                route: [ '', 'dashboard' ],
                name: 'dashboard',
                moduleId: PLATFORM.moduleName( './Modules/Dashboard/Dashboard' ),
                title: 'Dashboard'
            },
            {
                route: 'solutions',
                name: 'solutions',
                title: 'Solutions',
                moduleId: PLATFORM.moduleName( './Modules/Solutions/SolutionsRouter' )
            }
        ] );
    }
}
