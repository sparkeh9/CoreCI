import { observable } from 'aurelia-framework';
import {INavigationEntry } from './INavigationEntry';

export class NavigationViewModel
{
    @observable
    public name: string;
    @observable
    public navigationEntries: INavigationEntry[];
}
