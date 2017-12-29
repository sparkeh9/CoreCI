import { computedFrom } from 'aurelia-framework';
import {autoinject} from 'aurelia-dependency-injection';
import {EventAggregator} from 'aurelia-event-aggregator';
import {bindable, customElement} from 'aurelia-templating';

@customElement( 'aup-tabs' )
@autoinject
export class Tabs
{
    private element;
    private eventAggregator;
    
    @bindable
    public class = 'nav-tabs';
    @bindable
    public selectedTab;
    @bindable
    public tabs;

    constructor( element: Element, eventAggregator: EventAggregator )
    {
        this.element = element;
        this.eventAggregator = eventAggregator;
    }

    @computedFrom( 'tabs' )
    public get tabsToRender()
    {
        return Array.from(this.generateTabs());
    }

    private *generateTabs()
    {
        const tabs = this.tabs;
        
        for( let property in tabs )
        {
            if( tabs.hasOwnProperty( property ) )
            {
                if( this.tabs.hasOwnProperty( property ) )
                {
                    yield this.tabs[ property ];
                }
            }
        }
    }

    public attached(thing)
    {
        console.log( thing );
//        const active = this.tabs.find( tab => tab.active );
//        if( !active )
//        {
//            return;
//        }
//        document.querySelector( `#${ active.id }` ).classList.add( 'active' );
    }
    
    public click( event, tab )
    {
        this.selectedTab = tab;
        event.stopPropagation();
//        const target = event.target;
//        const active = this.element.querySelector( 'a.nav-link.active' );
//        if( target === active )
//        {
//            return;
//        }
//        const targetHref = target.getAttribute( 'href' );
//        const activeHref = active.getAttribute( 'href' );
//        target.classList.add( 'active' );
//        active.classList.remove( 'active' );
//        document.querySelector( targetHref ).classList.add( 'active' );
//        document.querySelector( activeHref ).classList.remove( 'active' );
//
//        this.selectedTab = tab;
        this.eventAggregator.publish( 'tabs:selected', tab );
    }
}
