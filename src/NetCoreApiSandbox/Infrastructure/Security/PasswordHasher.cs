namespace NetCoreApiSandbox.Infrastructure.Security
{
    #region

    using System;
    using System.Security.Cryptography;
    using System.Text;

    #endregion

    public sealed class PasswordHasher: IPasswordHasher, IDisposable
    {
        // Track whether Dispose has been called.
        private bool _disposed;
        private HMACSHA512 _hash = new HMACSHA512(Encoding.UTF8.GetBytes("ezpz_Lemon_SqzxD"));

        #region IDisposable Members

        /// <summary>
        /// Implements standard Dispose method of IDisposable interface
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IPasswordHasher Members

        /// <summary>
        /// Calculates pwd's hash
        /// </summary>
        /// <param name="password">Pwd string</param>
        /// <param name="salt">pwd's salt</param>
        /// <returns></returns>
        public byte[] Hash(string password, byte[] salt)
        {
            var bytes = Encoding.UTF8.GetBytes(password);

            var allBytes = new byte[bytes.Length + salt.Length];
            Buffer.BlockCopy(bytes, 0, allBytes, 0, bytes.Length);
            Buffer.BlockCopy(salt, 0, allBytes, bytes.Length, salt.Length);

            return this._hash.ComputeHash(allBytes);
        }

        #endregion

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                // Check to see if Dispose has already been called.
                if (disposing)
                {
                    // dispose managed resources
                    this._hash?.Dispose();
                }

                this._hash = null;

                // Note disposing has been done.
                this._disposed = true;
            }
        }

        /// <summary>
        /// Class destructor
        /// </summary>
        ~PasswordHasher()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            this.Dispose(false);
        }
    }
}
