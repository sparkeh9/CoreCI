import { PLATFORM } from 'aurelia-framework';

export function configure( config:any )
{
    config.globalResources( PLATFORM.moduleName( './breadcrumbs/Breadcrumbs' ) );
}
