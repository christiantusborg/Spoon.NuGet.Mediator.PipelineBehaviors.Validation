namespace Spoon.NuGet.Mediator.PipelineBehaviors.Validation
{
    using Assistants;
    using EitherCore.PipelineBehaviors;
    using FluentValidation;
    using Interceptors.LogInterceptor;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    ///     Class ServiceCollectionExtensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds the validation pipeline behaviour.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddValidationPipelineBehaviour(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            services.AddValidatorsFromAssemblies(assemblies);

            if (services.Any(d => d.ImplementationType == typeof(EitherPipelineBehavior<,>)))
            {
                throw new InvalidOperationException("EitherPipelineBehavior<,> is was registered before ValidationPipelineBehaviour<,>");
            }

            if (services.Any(d => d.ImplementationType == typeof(ValidationPipelineBehaviour<,>)))
            {
                throw new InvalidOperationException("ValidationPipelineBehaviour<,> is was registered already");
            }

            services.AddInterceptedSingleton<IValidationPipelineAssistant, ValidationPipelineAssistant, LogInterceptorDefault>();

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EitherPipelineBehavior<,>));

            return services;
        }
    }
}