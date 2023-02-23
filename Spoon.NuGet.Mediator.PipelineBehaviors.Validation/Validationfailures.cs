namespace Spoon.NuGet.Mediator.PipelineBehaviors.Validation
{

    /// <summary>
    /// 
    /// </summary>
    public class Validationfailures
    {
        /// <inheritdoc />
        public string Command { get; set; } = String.Empty;

        /// <inheritdoc />
        public Dictionary<string, string> CommandValues { get; set; } = new Dictionary<string, string>();
        
        /// <inheritdoc />
        public string Origin { get; set; } = String.Empty;
        
        /// <inheritdoc />
        public int HttpStatusCode { get; set; } = 0;
        
        /// <inheritdoc />
        public string Message { get; set; } = String.Empty;

        /// <inheritdoc />
        public Validationfailure[] ValidationFailures { get; set; } = new Validationfailure[] {};
    }

    

    /// <summary>
    /// 
    /// </summary>
    public class Validationfailure
    {
        /// <inheritdoc />
        public string propertyName { get; set; }  = String.Empty;
        
        /// <inheritdoc />
        public string errorMessage { get; set; } = String.Empty;
        
        /// <inheritdoc />
        public object attemptedValue { get; set; } = String.Empty;
        
        /// <inheritdoc />
        public object customState { get; set; } = String.Empty;

        /// <inheritdoc />
        public int severity { get; set; } = 0;
        
        /// <inheritdoc />
        public string errorCode { get; set; }  = String.Empty;

        /// <inheritdoc />
        public Formattedmessageplaceholdervalues formattedMessagePlaceholderValues { get; set; } = new Formattedmessageplaceholdervalues();
    }

    /// <summary>
    /// 
    /// </summary>
    public class Formattedmessageplaceholdervalues
    {
        /// <inheritdoc />
        public string PropertyName { get; set; }  = String.Empty;
        
        /// <inheritdoc />
        public object PropertyValue { get; set; }  = String.Empty;
    }
    
}