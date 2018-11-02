using System;
using System.ComponentModel.DataAnnotations;

namespace iKudo.Clients.Web.Validation
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private string PropertyName { get; set; }
        private object DesiredValue { get; set; }
        private readonly RequiredAttribute innerAttribute;

        public RequiredIfAttribute(String propertyName, Object desiredvalue)
        {
            PropertyName = propertyName;
            DesiredValue = desiredvalue;
            innerAttribute = new RequiredAttribute();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dependentValue = validationContext.ObjectInstance.GetType()
                                                                 .GetProperty(PropertyName)
                                                                 .GetValue(validationContext.ObjectInstance, null);

            if (dependentValue?.ToString() == DesiredValue?.ToString() && !innerAttribute.IsValid(value))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}