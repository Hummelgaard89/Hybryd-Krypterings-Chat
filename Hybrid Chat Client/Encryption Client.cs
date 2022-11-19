using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Hybrid_Chat_Client
{
    public class Encryption_Client
    {
        public string RsaPublicKey { get; private set; }
        public string RsaPrivateKey { get; private set; }
        public Encryption_Client()
        {
            /*RsaPublicKey = */GetPublicKey();
            //RsaPrivateKey = rsaPrivateKey;
        }

        private static RSACryptoServiceProvider rsaSession = new RSACryptoServiceProvider(2048);

        private static string GetPublicKey()
        {
            return rsaSession.ToXmlString(false);
        }
    }
}
