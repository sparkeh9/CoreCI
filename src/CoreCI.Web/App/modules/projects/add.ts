import { autoinject } from 'aurelia-framework';
import { RouterConfiguration, Router } from 'aurelia-router';

@autoinject
export default class Projects
{
    private readonly myTabValues: any;

    constructor()
    {
        this.myTabValues = [
            { id: 'section-one', label: 'My First Section', selected: true },
            { id: 'section-two', label: 'Users' },
            { id: 'section-three', label: 'Browse Items' }
        ];
    }
}
