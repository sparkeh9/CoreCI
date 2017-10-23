require('sass/main.scss');
import $ from 'jquery';
import 'popper.js';
import { Aurelia, PLATFORM, TemplatingEngine} from 'aurelia-framework';

export function configure( aurelia: Aurelia )
{
    aurelia.use
        .standardConfiguration()
        .plugin( PLATFORM.moduleName( 'aurelia-validation' ) )
        .plugin( PLATFORM.moduleName( 'aurelia-computed' ), { enableLogging: true } )
        .developmentLogging();

    aurelia.start()
        .then( a =>
        {
            const templatingEngine: TemplatingEngine = a.container.get(TemplatingEngine);
//            templatingEngine.enhance({
//                container: a.container,
//                element: document.querySelector('my-component'),
//                resources: a.resources
//            });

//            return aurelia.setRoot( PLATFORM.moduleName( 'app' ) );
        } );
//           .then(() => aurelia.setRoot(PLATFORM.moduleName('app')));
}
