namespace NetCoreApiSandbox.Infrastructure
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentValidation;
    using MediatR;

    #endregion

    public class ValidationPipelineBehavior<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            this._validators = validators;
        }

        #region IPipelineBehavior<TRequest,TResponse> Members

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext(request);

            var failures = this._validators.Select(v => v.Validate(context))
                               .SelectMany(result => result.Errors)
                               .Where(f => f != null)
                               .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }

            return await next();
        }

        #endregion
    }
}
