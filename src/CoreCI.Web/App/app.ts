import { noView, InlineViewStrategy } from 'aurelia-framework';

export class AppViewModel
{
    private readonly text: string;
    constructor() {
        this.text = "Hello world";
    }

    greet() {
        alert('test');
    }

    getViewStrategy()
    {
        return new InlineViewStrategy(document.getElementById('AppView').innerHTML);
    }
}
