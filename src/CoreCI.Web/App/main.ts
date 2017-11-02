require( 'sass/main.scss' );
import { Aurelia, PLATFORM, } from 'aurelia-framework';

export async function configure( aurelia: Aurelia )
{
    aurelia.use
        .standardConfiguration()
        .developmentLogging()
        .feature(PLATFORM.moduleName('components/index'));

    await aurelia.start();
    aurelia.setRoot(PLATFORM.moduleName('app'));
}