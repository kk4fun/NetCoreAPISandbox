namespace NetCoreApiSandbox.IntegrationTests
{
    #region

    using NetCoreApiSandbox.Infrastructure;

    #endregion

    public class StubCurrentUserAccessor: ICurrentUserAccessor
    {
        private readonly string _currentUserName;

        /// <summary>
        ///     stub the ICurrentUserAccessor with a given userName to be used in tests
        /// </summary>
        /// <param name="userName">Current user's name</param>
        public StubCurrentUserAccessor(string userName)
        {
            this._currentUserName = userName;
        }

        #region ICurrentUserAccessor Members

        public string GetCurrentUsername()
        {
            return this._currentUserName;
        }

        #endregion
    }
}
