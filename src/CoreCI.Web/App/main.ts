require('sass/main.scss');

import { Aurelia, PLATFORM } from 'aurelia-framework';
import { ValidationControllerFactory, ValidationController, validationMessages, ValidationRules } from 'aurelia-validation';
declare var module: any;

export function configure( aurelia: Aurelia )
{
    aurelia.use
        .standardConfiguration()
        .plugin( PLATFORM.moduleName( 'aurelia-validation' ) )
        .plugin( PLATFORM.moduleName( 'aurelia-computed' ), { enableLogging: true } )
        .developmentLogging();

    aurelia.start()
        .then(() => aurelia.setRoot(PLATFORM.moduleName('app')));
}
