namespace CoreCI.Common.Data.Repository.Model.Projects
{
    using FluentValidation;
    using Infrastructure.Validation;

    public class GetProjectsQueryValidator : AbstractValidator<GetProjectsQuery>
    {
        public GetProjectsQueryValidator()
        {
            RuleFor( x => x.Page )
                .GreaterThanOrEqualTo( 1 )
                .WithMessage( "Must have a positive page number" );
            RuleFor( x => x.Solution )
                .ObjectId()
                .WithMessage( "'{PropertyName}' must be a valid ID" );
        }
    }
}
