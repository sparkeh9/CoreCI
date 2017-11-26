require( 'sass/main.scss' );
import { Aurelia, PLATFORM, } from 'aurelia-framework';

export async function configure( aurelia: Aurelia )
{
    aurelia.use
        .standardConfiguration()
        .developmentLogging()
        .plugin( PLATFORM.moduleName( 'aurelia-validation' ) )
        .plugin( PLATFORM.moduleName( 'aurelia-plugins-tabs' ) )
        .feature( PLATFORM.moduleName( 'Components/Index' ) );

    await aurelia.start();
    aurelia.setRoot( PLATFORM.moduleName( 'App' ) );
}
