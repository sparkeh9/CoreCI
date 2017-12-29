import * as NavigationEntrySettings from './INavigationEntrySettings';

export interface INavigationEntry
{
    active: boolean;
    text: string;
    url: string;
    children: INavigationEntry[];
    settings: NavigationEntrySettings.INavigationEntrySettings;
}