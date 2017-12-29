namespace CoreCI.Common.Infrastructure.Validation
{
    using CoreCI.Common.Extensions;
    using FluentValidation.Validators;
    using MongoDB.Bson;

    public class ObjectIdValidator : PropertyValidator
    {
        public ObjectIdValidator() : base( "'{PropertyName}' must be a valid ID." ) { }

        protected override bool IsValid( PropertyValidatorContext context )
        {
            var value = context.PropertyValue;

            return value == null ||
                   value.ToString().IsNullOrWhiteSpace() ||
                   ObjectId.TryParse( value.ToString(), out ObjectId _ );
        }
    }
}
