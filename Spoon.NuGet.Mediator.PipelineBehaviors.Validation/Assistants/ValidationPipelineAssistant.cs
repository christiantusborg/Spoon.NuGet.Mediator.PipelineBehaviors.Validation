namespace Spoon.NuGet.Mediator.PipelineBehaviors.Validation.Assistants;

using EitherCore;
using EitherCore.Enums;
using EitherCore.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Interceptors.LogInterceptor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

/// <summary>
/// Class ValidationPipelineAssistant.
/// Implements the <see cref="IValidationPipelineAssistant" />.
/// </summary>
/// <seealso cref="IValidationPipelineAssistant" />
[LogInterceptorDefaultLogLevel(LogLevel.None)]
public class ValidationPipelineAssistant : IValidationPipelineAssistant
{
    /// <summary>
    /// Determines whether the specified validators has validators.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    /// <param name="validators">The validators.</param>
    /// <returns><c>true</c> if the specified validators has validators; otherwise, <c>false</c>.</returns>
    public bool HasValidators<TRequest>(IEnumerable<IValidator<TRequest>> validators)
    {
        return !validators.Any();
    }

    /// <summary>
    /// Gets the failed validate.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    /// <param name="request">The request.</param>
    /// <param name="validators">The validators.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;List&lt;ValidationFailure&gt;&gt;.</returns>
    public async Task<List<ValidationFailure>> GetFailedValidate<TRequest>(TRequest request, IEnumerable<IValidator<TRequest>> validators, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var validationResults =
            await Task.WhenAll(validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var failures = validationResults.SelectMany(result => result.Errors).Where(failure => failure != null).ToList();

        return failures;
    }

    /// <summary>
    /// Determines whether [has any validation failures] [the specified failures validate].
    /// </summary>
    /// <param name="failuresValidate">The failures validate.</param>
    /// <returns><c>true</c> if [has any validation failures] [the specified failures validate]; otherwise, <c>false</c>.</returns>
    public bool HasAnyValidationFailures(List<ValidationFailure> failuresValidate)
    {
        return failuresValidate.Count != 0;
    }

    /// <summary>
    /// Determines whether [is not either] [the specified type either].
    /// </summary>
    /// <typeparam name="TResponse">The type of the t response.</typeparam>
    /// <param name="typeEither">The type either.</param>
    /// <returns><c>true</c> if [is not either] [the specified type either]; otherwise, <c>false</c>.</returns>
    public bool IsNotEither<TResponse>(out Type? typeEither)
    {
        var responseType = typeof(TResponse);

        if (responseType.GetGenericTypeDefinition() != typeof(Either<>))
        {
            typeEither = null;
            return false;
        }

        typeEither = responseType;
        return true;
    }

    /// <summary>
    /// Validations the failures to dictionary.
    /// </summary>
    /// <param name="failuresValidate">The failures validate.</param>
    /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
    public Dictionary<string, object> ValidationFailuresToDictionary(List<ValidationFailure> failuresValidate)
    {
        var result = new Dictionary<string, object> { { "ValidationFailures", failuresValidate } };

        return result;
    }

    /// <summary>
    /// Gets the validation pipeline behaviour configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="validationPipelineConfigOptions">The validation pipeline configuration options.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool GetValidationPipelineBehaviourConfig(IConfiguration? configuration, out IConfigurationSection? validationPipelineConfigOptions)
    {
        if (configuration is null)
        {
            validationPipelineConfigOptions = null;
            return false;
        }

        validationPipelineConfigOptions = configuration.GetSection("ValidationPipeline:ValidationFailedHttpStatusCode");

        return validationPipelineConfigOptions is not null;
    }

    /// <summary>
    /// Gets the validation pipeline behaviour configuration HTTP code.
    /// </summary>
    /// <param name="validationPipelineConfigOptions">The validation pipeline configuration options.</param>
    /// <param name="httpStatusCodes">The HTTP status codes.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool GetValidationPipelineBehaviourConfigHttpCode(IConfigurationSection? validationPipelineConfigOptions, out BaseHttpStatusCodes httpStatusCodes)
    {
        return Enum.TryParse(validationPipelineConfigOptions!.Value, out httpStatusCodes);
    }

    /// <summary>
    /// Creates the either exception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    /// <param name="request">The request.</param>
    /// <param name="origin">The origin.</param>
    /// <param name="message">The message.</param>
    /// <param name="httpStatusCodes">The HTTP status codes.</param>
    /// <param name="validationFailuresAsDictionary">The validation failures as dictionary.</param>
    /// <returns>EitherException.</returns>
    public EitherException CreateEitherException<TRequest>(TRequest request, string origin, string message, BaseHttpStatusCodes httpStatusCodes, Dictionary<string, object> validationFailuresAsDictionary)
    {
        var ext = new EitherException(request!, string.Empty, message, httpStatusCodes, validationFailuresAsDictionary);
        return ext;
    }

    /// <summary>
    /// Creates the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the t response.</typeparam>
    /// <param name="responseType">Type of the response.</param>
    /// <param name="ext">The ext.</param>
    /// <returns>TResponse.</returns>
    public TResponse CreateResponse<TResponse>(Type? responseType, EitherException ext)
    {
        var response = (TResponse)Activator.CreateInstance(responseType!, ext) !;
        return response;
    }
}