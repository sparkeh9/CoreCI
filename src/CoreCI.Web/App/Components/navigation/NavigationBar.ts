import {autoinject, containerless, customElement} from 'aurelia-framework';
import {bindable, bindingMode, BindingEngine} from 'aurelia-framework';
import {NavigationViewModel} from '../../Models/NavigationViewModel';

@autoinject
@containerless
@customElement( 'navigation-bar' )
export class NavigationBar
{
    private readonly bindingEngine:BindingEngine;

    @bindable
    public navigation: NavigationViewModel;

    constructor( bindingEngine: BindingEngine )
    {
        this.bindingEngine = bindingEngine;
    }
//
//    public bind()
//    {
//        this.navigationViewModel = new NavigationViewModel();
//        this.navigationViewModel.name = 'default';
//    }
}
