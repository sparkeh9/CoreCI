export class BreadcrumbFilterValueConverter
{
    toView( array )
    {
        if( array == null )
            return null;

        return array.filter( x =>
        {
            if( !x.config.hasOwnProperty( 'settings' ) )
                return true;

            if( !x.config.settings.hasOwnProperty( 'hideFromBreadcrumb' ) )
                return true;

            return !x.config.settings.hideFromBreadcrumb;
        } );
    }
}
