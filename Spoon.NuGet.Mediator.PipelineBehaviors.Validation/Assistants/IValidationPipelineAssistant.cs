namespace Spoon.NuGet.Mediator.PipelineBehaviors.Validation.Assistants;

using EitherCore.Enums;
using EitherCore.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Interface IValidationPipelineSubstructure.
/// </summary>
public interface IValidationPipelineAssistant
{
    /// <summary>
    /// Determines whether the specified validators has validators.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    /// <param name="validators">The validators.</param>
    /// <returns><c>true</c> if the specified validators has validators; otherwise, <c>false</c>.</returns>
    public bool HasValidators<TRequest>(IEnumerable<IValidator<TRequest>> validators);

    /// <summary>
    /// Gets the failed validate.
    /// </summary>
    /// <typeparam name="TRequest">The type of the t request.</typeparam>
    /// <param name="request">The request.</param>
    /// <param name="validators">The validators.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Task&lt;List&lt;ValidationFailure&gt;&gt;.</returns>
    public Task<List<ValidationFailure>> GetFailedValidate<TRequest>(TRequest request, IEnumerable<IValidator<TRequest>> validators, CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether [has any validation failures] [the specified failures validate].
    /// </summary>
    /// <param name="failuresValidate">The failures validate.</param>
    /// <returns><c>true</c> if [has any validation failures] [the specified failures validate]; otherwise, <c>false</c>.</returns>
    bool HasAnyValidationFailures(List<ValidationFailure> failuresValidate);

    /// <summary>
    /// Determines whether [is not either] [the specified type either].
    /// </summary>
    /// <typeparam name="TResponse">The type of the t response.</typeparam>
    /// <param name="typeEither">The type either.</param>
    /// <returns><c>true</c> if [is not either] [the specified type either]; otherwise, <c>false</c>.</returns>
    public bool IsNotEither<TResponse>(out Type? typeEither);

    /// <summary>
    /// Validations the failures to dictionary.
    /// </summary>
    /// <param name="failuresValidate">The failures validate.</param>
    /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
    Dictionary<string, object> ValidationFailuresToDictionary(List<ValidationFailure> failuresValidate);

    /// <summary>
    /// Gets the validation pipeline behaviour configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="validationPipelineConfigOptions">The validation pipeline configuration options.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool GetValidationPipelineBehaviourConfig(IConfiguration? configuration, out IConfigurationSection? validationPipelineConfigOptions);

    /// <summary>
    /// Gets the validation pipeline behaviour configuration HTTP code.
    /// </summary>
    /// <param name="validationPipelineConfigOptions">The validation pipeline configuration options.</param>
    /// <param name="httpStatusCodes">The HTTP status codes.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool GetValidationPipelineBehaviourConfigHttpCode(IConfigurationSection? validationPipelineConfigOptions, out BaseHttpStatusCodes httpStatusCodes);

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
    EitherException CreateEitherException<TRequest>(TRequest request, string origin, string message, BaseHttpStatusCodes httpStatusCodes, Dictionary<string, object> validationFailuresAsDictionary);

    /// <summary>
    /// Creates the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the t response.</typeparam>
    /// <param name="responseType">Type of the response.</param>
    /// <param name="ext">The ext.</param>
    /// <returns>TResponse.</returns>
    TResponse CreateResponse<TResponse>(Type? responseType, EitherException ext);
}