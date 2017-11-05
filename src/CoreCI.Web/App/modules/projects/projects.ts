import { autoinject } from 'aurelia-framework';
import { Router } from 'aurelia-router';

@autoinject
export default class Projects
{
    private readonly router: Router;

    constructor( router: Router )
    {
        this.router = router;
    }

    public addProject(): void
    {
        this.router.navigate( 'projects/add' );
    }
}
