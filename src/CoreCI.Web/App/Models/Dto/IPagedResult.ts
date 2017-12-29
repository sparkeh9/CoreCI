export interface IPagedResult<T>
{
    page: number;
    next: string;
    values: T[];
}
