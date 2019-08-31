using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using Microsoft.Extensions.Options;


namespace BindingAttributes {

    public class BindingAttributesValidateOptions<TOptions> : IValidateOptions<TOptions> where TOptions : class {

        public BindingAttributesValidateOptions(string name, OptionsAttribute attr) {
            Name = name;
            Attr = attr;
        }

        public string Name { get; }
        public OptionsAttribute Attr { get; }

        private string ReportForValidationResult(ValidationResult result) {
            var path = Attr.Section;
            var members = string.Join(", ", result.MemberNames);

            return
                $"Configuration failure in section '{path}' for member '{members}' with message: {result.ErrorMessage}";

        }

        public ValidateOptionsResult Validate(string name, TOptions options) {
            if (Name != null && name != Name)
                return ValidateOptionsResult.Skip;

            List<ValidationResult> source = new List<ValidationResult>();

            var validationContext = new ValidationContext(options, null, null);
            
            if (Validator.TryValidateObject(options, validationContext, source, true))
                return ValidateOptionsResult.Success;

            var failures = source.Select(ReportForValidationResult);
            var message = string.Join(Environment.NewLine, failures);
            return ValidateOptionsResult.Fail(message);
        }

    }

}