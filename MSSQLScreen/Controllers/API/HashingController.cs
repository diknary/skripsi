using MSSQLScreen.Models;
using System;
using System.Security.Cryptography;
using System.Web.Http;

namespace MSSQLScreen.Controllers.API
{
    
    public class HashingController : ApiController
    {
        private ApplicationDbContext _context;

        public HashingController()
        {
            _context = new ApplicationDbContext();
        }

        private static byte[] CreateRandomSalt(int length)
        {

            byte[] randBytes;

            if (length >= 1)
            {
                randBytes = new byte[length];
            }
            else
            {
                randBytes = new byte[1];
            }

            RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();

            rand.GetBytes(randBytes);

            return randBytes;
        }

        private static void ClearBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentException("buffer");
            }

            for (int x = 0; x < buffer.Length; x++)
            {
                buffer[x] = 0;
            }
        }

        [HttpPost]
        [Route("api/newuser")]
        public void NewUser(AddUserViewModel user)
        {
            byte[] salt = CreateRandomSalt(10);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

            try
            {
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(user.Password, salt);

                tdes.Key = pdb.CryptDeriveKey("TripleDES", "SHA1", 192, tdes.IV);

                var newUser = new UserAccount
                {
                    Name = user.Name,
                    Username = user.Username,
                    Password = Convert.ToBase64String(tdes.Key),
                    Salt = Convert.ToBase64String(salt)
                };
                _context.UserAccounts.Add(newUser);
                _context.SaveChanges();
            }
            finally
            {
                ClearBytes(salt);
                tdes.Clear();
            }
        }
    }
}
