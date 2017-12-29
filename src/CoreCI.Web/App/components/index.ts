import { PLATFORM } from 'aurelia-framework';

export function configure( config: any )
{
    config.globalResources( [
        PLATFORM.moduleName( './breadcrumbs/Breadcrumbs' ),
        PLATFORM.moduleName( './navigation/NavigationBar' ),
        PLATFORM.moduleName('./tabs/aurelia-plugins-tabs-tabs'),
        PLATFORM.moduleName('./tabs/aurelia-plugins-tabs-tab-content'),
        PLATFORM.moduleName('./tabs/aurelia-plugins-tabs-tab-pane')
    ] );
}
