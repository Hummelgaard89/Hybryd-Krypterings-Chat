using System;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hybrid_Chat
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener server = null;
            try
            {
                int portNumber = 7;
                server = new TcpListener(portNumber);
                server.Start();
                Console.WriteLine("Echo server running on port 7.");

                //RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
                //string publicKey = csp.ToXmlString(false);
                //string privateKey = csp.ToXmlString(true);
                //string passPhrase = "AESKEYANDIV";

                //SymmetricAlgorithm aesSession = Aes.Create();
                //aesSession.KeySize = 256;
                //byte[] aesKey = aesSession.Key;
                //string aeskeystring = Convert.ToBase64String(aesKey);
                //byte[] aesIV = aesSession.IV;
                //string aesivstring = Convert.ToBase64String(aesIV);

                //byte[] combinedAESKey = Encoding.UTF8.GetBytes(passPhrase + aeskeystring);
                //byte[] combinedAESIV = Encoding.UTF8.GetBytes(passPhrase  + aesivstring);


                //csp.FromXmlString(publicKey);

                //byte[] encryptedCombinedAESKey = csp.Encrypt(combinedAESKey, true);
                //byte[] encryptedCombinedIV = csp.Encrypt(combinedAESIV, true);

                //csp.FromXmlString(privateKey);
                
                //byte[] decryptedCombinedAESKey = csp.Decrypt(encryptedCombinedAESKey, true);
                //byte[] decryptedCombinedIV = csp.Decrypt(encryptedCombinedIV, true);

                //string decryptedCombinedAESKeyString = Encoding.UTF8.GetString(decryptedCombinedAESKey);
                //string decryptedCombinedIVString = Encoding.UTF8.GetString(decryptedCombinedIV);

                ////Console.WriteLine("KEY BEFORE: " + aeskeystring + "\nIV BEFORE: " + aesivstring);
                //string decryptedAESKeyClean = decryptedCombinedAESKeyString.Remove(0,11);
                //string decryptedAESIVClean = decryptedCombinedIVString.Remove(0, 11);

                //Console.WriteLine("KEY AFTER: " + decryptedAESKeyClean + "\nIV AFTER: " + decryptedAESIVClean);

                while (true)
                {
                    EchoServer es = new EchoServer(server.AcceptTcpClient());
                    Thread serverThread = new Thread(
                        new ThreadStart(es.Conversation));
                    serverThread.Start();
                }
            }
            catch (Exception e) 
            {
                Console.WriteLine(e + " " + e.StackTrace);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
