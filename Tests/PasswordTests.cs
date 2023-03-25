using dionizos_backend_app.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class PasswordTests
    {
        [Theory]
        [InlineData("Zeus", "Dionizos")]
        public void DifferentPasswords(string s1, string s2)
        {
            Assert.NotEqual(Extensions.EncryptPass(s1), Extensions.EncryptPass(s2));
        }

        [Theory]
        [InlineData("Dionizos", "Dionizos")]
        public void SamePassword(string s1, string s2)
        {
            Assert.Equal(Extensions.EncryptPass(s1), Extensions.EncryptPass(s2));
        }

        [Fact]
        public void VeryLongPasswords()
        {
            Random random = new Random();
            const int length = 300;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string s1 = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            string s2 = s1 + "d";
            DifferentPasswords(s1, s2);
        }
    }
}
