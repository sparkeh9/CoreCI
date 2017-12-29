import { autoinject } from 'aurelia-framework';
import { EventAggregator } from 'aurelia-event-aggregator';

@autoinject
export default class Dashboard
{
    private readonly eventAggregator: EventAggregator;

    constructor( eventAggregator: EventAggregator )
    {
        this.eventAggregator = eventAggregator;
    }

    public attached()
    {
        this.eventAggregator.publish( 'CoreCI:NavigationUpdated', null );
    }
}

