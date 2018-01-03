// IMPORTS
import {bindable, containerless, customElement} from 'aurelia-templating';


// CLASS ATTRIBUTES
@containerless()
@customElement('aup-tab-pane')


// PUBLIC CLASS
export class TabPane {
  // BINDABLE PROPERTIES
  @bindable model;
  @bindable tab;
  @bindable selectedTab;
  @bindable viewModel;
}