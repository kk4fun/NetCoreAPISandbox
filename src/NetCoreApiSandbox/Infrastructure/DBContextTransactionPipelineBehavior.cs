namespace NetCoreApiSandbox.Infrastructure
{
    #region

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    #endregion

    public class DbContextTransactionPipelineBehavior<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse>
    {
        private readonly NetCoreSandboxApiContext _context;

        public DbContextTransactionPipelineBehavior(NetCoreSandboxApiContext context)
        {
            this._context = context;
        }

        #region IPipelineBehavior<TRequest,TResponse> Members

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            TResponse result = default;

            try
            {
                this._context.BeginTransaction();

                result = await next();

                this._context.CommitTransaction();
            }
            catch (Exception)
            {
                this._context.RollbackTransaction();

                throw;
            }

            return result;
        }

        #endregion
    }
}
