using System;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace BindingAttributes {

    public static class BindingAttributesOptionsBuilderExtensions {

        public static OptionsBuilder<TOptions> ValidateOptionsAnnotations<TOptions>(
            this OptionsBuilder<TOptions> optionsBuilder)
            where TOptions : class {
            
            Console.WriteLine($"Calling ValidateOptionsAnnotations: {typeof(TOptions)}");

            var optionsType = typeof(TOptions);
            var optionsAttrs = optionsType.GetCustomAttributes<OptionsAttribute>();

            if (!optionsAttrs.Any()) {
                return optionsBuilder;
            }

            var optionsAttr = optionsAttrs.First();
            var instance = new BindingAttributesValidateOptions<TOptions>(optionsBuilder.Name, optionsAttr);

            optionsBuilder.Services.AddSingleton((IValidateOptions<TOptions>) instance);

            Console.WriteLine($"Completed ValidateOptionsAnnotations");
            return optionsBuilder;
        }

    }

}