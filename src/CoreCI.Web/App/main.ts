require( 'sass/main.scss' );
import { Aurelia, PLATFORM, } from 'aurelia-framework';

export async function configure( aurelia: Aurelia )
{
    delete (<any>window).webkitStorageInfo;
    delete (<any>window).webkitIndexedDB;

    aurelia.use
        .standardConfiguration()
        //.developmentLogging()
        .plugin( PLATFORM.moduleName( 'aurelia-validation' ) )
//        .plugin( PLATFORM.moduleName( 'aurelia-plugins-tabs' ) )
        .feature( PLATFORM.moduleName( 'Components/Index' ) );

    await aurelia.start();
    aurelia.setRoot( PLATFORM.moduleName( 'App' ) );
}
