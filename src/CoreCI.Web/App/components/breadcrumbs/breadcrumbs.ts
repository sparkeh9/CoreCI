import {autoinject, PLATFORM, containerless} from 'aurelia-framework';
import {Router} from 'aurelia-router';

@autoinject
@containerless
export class Breadcrumbs
{
    private readonly router: Router;

    public constructor( router: Router )
    {
        while( router.parent )
        {
            router = router.parent;
        }
        this.router = router;
    }

    public navigate( navigationInstruction )
    {
        console.log( 'Navigate to:', navigationInstruction );
        navigationInstruction.router.navigateToRoute( navigationInstruction.config.name, navigationInstruction.params );
    }
}
