namespace NetCoreApiSandbox.Infrastructure.Security
{
    #region

    using System;
    using System.Security.Cryptography;
    using System.Text;

    #endregion

    public class PasswordHasher: IPasswordHasher, IDisposable
    {
        private readonly HMACSHA512 x = new HMACSHA512(Encoding.UTF8.GetBytes("realworld"));

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IPasswordHasher Members

        public byte[] Hash(string password, byte[] salt)
        {
            var bytes = Encoding.UTF8.GetBytes(password);

            var allBytes = new byte[bytes.Length + salt.Length];
            Buffer.BlockCopy(bytes, 0, allBytes, 0, bytes.Length);
            Buffer.BlockCopy(salt, 0, allBytes, bytes.Length, salt.Length);

            return this.x.ComputeHash(allBytes);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                this.x?.Dispose();
            }
        }
    }
}
