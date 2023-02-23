namespace Spoon.NuGet.Mediator.PipelineBehaviors.Validation;

using Assistants;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Class ValidationPipelineBehaviour.
/// Implements the <see cref="IPipelineBehavior{TRequest,TResponse}" />.
/// </summary>
/// <typeparam name="TRequest">The type of the t request.</typeparam>
/// <typeparam name="TResponse">The type of the t response.</typeparam>
/// <seealso cref="IPipelineBehavior{TRequest, TResponse}" />
public class ValidationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// The validators.
    /// </summary>
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// The substructure.
    /// </summary>
    private readonly IValidationPipelineAssistant _assistant;

    /// <summary>
    /// The configuration.
    /// </summary>
    private readonly IConfiguration? _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationPipelineBehaviour{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="validators">The validators.</param>
    /// <param name="assistant">The substructure.</param>
    /// <param name="configuration">The configuration.</param>
    public ValidationPipelineBehaviour(IEnumerable<IValidator<TRequest>> validators, IValidationPipelineAssistant assistant, IConfiguration? configuration)
    {
        this._validators = validators;
        this._assistant = assistant;
        this._configuration = configuration;
    }

    /// <summary>
    /// Pipeline handler. Perform any additional behavior and await the <paramref name="next" /> delegate as necessary.
    /// </summary>
    /// <param name="request">Incoming request.</param>
    /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Awaitable task returning the <typeparamref name="TResponse" />Validation error responce.</returns>
    /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">When Error is not an Either.</exception>
    /// <exception cref="System.ArgumentNullException">ValidationPipelineBehaviour -> ValidationPipeline:ValidationFailedHttpStatusCode missing.</exception>
    /// <exception cref="System.ArgumentNullException">ValidationPipelineBehaviour -> ValidationPipeline:ValidationFailedHttpStatusCode invalid BaseHttpStatusCodes.</exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (this._assistant.HasValidators(this._validators)) return await next();

        var failuresValidate = await this._assistant.GetFailedValidate(request, this._validators, cancellationToken);

        if (!this._assistant.HasAnyValidationFailures(failuresValidate)) 
            return await next();

        if (!this._assistant.IsNotEither<TResponse>(out var responseType))
            throw new ValidationException(failuresValidate);

        var validationFailuresAsDictionary = this._assistant.ValidationFailuresToDictionary(failuresValidate);

        if (!this._assistant.GetValidationPipelineBehaviourConfig(this._configuration, out var validationPipelineConfigOptions))
            throw new ArgumentNullException($"ValidationPipelineBehaviour -> ValidationPipeline:ValidationFailedHttpStatusCode missing");

        if (!this._assistant.GetValidationPipelineBehaviourConfigHttpCode(validationPipelineConfigOptions, out var httpStatusCodes))
            throw new ArgumentNullException($"ValidationPipelineBehaviour -> ValidationPipeline:ValidationFailedHttpStatusCode invalid BaseHttpStatusCodes");

        var ext = this._assistant.CreateEitherException(request, string.Empty, "ValidationFailure", httpStatusCodes, validationFailuresAsDictionary);

        var response = this._assistant.CreateResponse<TResponse>(responseType, ext);

        return response;
    }
}