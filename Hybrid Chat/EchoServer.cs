using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using System.Diagnostics;

namespace Hybrid_Chat
{
    public class EchoServer
    {
        StreamReader sr = null;
        StreamWriter sw = null;
        TcpClient client= null;
        SymmetricAlgorithm aesSession = Aes.Create();


        public EchoServer(TcpClient tcpClient) { client = tcpClient; }

        public void Conversation()
        {
            try
            {
                Console.WriteLine("Connection Accepted");

                sr = new StreamReader(client.GetStream());

                sw = new StreamWriter(client.GetStream());

                string incoming = sr.ReadLine();
                while (incoming != null)
                {
                    if (incoming.StartsWith("Encrypt") == true)
                    {
                        EncryptionHandshake();
                        while (DecryptCommunication(incoming, aesSession) != "Exit")
                        {
                            Console.WriteLine("Message recieved: " + DecryptCommunication(incoming, aesSession));
                            sw.WriteLine(EncryptCommunication(incoming, aesSession));
                            sw.Flush();
                            Console.WriteLine("Message sent back: " + EncryptCommunication(incoming, aesSession));
                            incoming = sr.ReadLine();
                        }


                    }
                    else
                    { 
                    Console.WriteLine("Message recieved: " + incoming);
                    sw.WriteLine(incoming);
                    sw.Flush();
                    Console.WriteLine("Message sent back: " + incoming);
                    incoming = sr.ReadLine();
                    }
                }

                Console.WriteLine("Client sent 'Exit': closing connection.");
                sr.Close();
                sw.Close();
                client.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e + " " + e.StackTrace);
            }
            finally
            {
                if (sr != null) sr.Close();
                if (sr != null) sw.Close();
                if (client != null) client.Close();
            }
        }
        public void EncryptionHandshake()
        {
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
            string publicKey = sr.ReadLine();
            string publicKeyClean = publicKey.Remove(0, 7);
            string passPhraseKey = "AESKEYANDIVKEY";
            string passPhraseIV = "AESKEYANDIVIV";


            aesSession.KeySize = 256;
            byte[] aesKey = aesSession.Key;
            string aeskeystring = Convert.ToBase64String(aesKey);
            byte[] aesIV = aesSession.IV;
            string aesivstring = Convert.ToBase64String(aesIV);
            byte[] combinedAESKey = Convert.FromBase64String(passPhraseKey + aeskeystring);
            byte[] combinedAESIV = Convert.FromBase64String(passPhraseIV + aesivstring);


            csp.FromXmlString(publicKeyClean);
            byte[] encryptedCombinedAESKey = csp.Encrypt(combinedAESKey, true);
            byte[] encryptedCombinedIV = csp.Encrypt(combinedAESIV, true);
            sw.WriteLine(encryptedCombinedAESKey);
            sw.WriteLine(encryptedCombinedIV);
            Console.WriteLine(encryptedCombinedAESKey);
            Console.WriteLine(encryptedCombinedIV);
        }
        public static string EncryptCommunication(string keyInput, SymmetricAlgorithm symmetricAlgorithm)
        {
            SymmetricAlgorithm sa = symmetricAlgorithm;
            byte[] encryptedBytes;
            ICryptoTransform encrypting = sa.CreateEncryptor(sa.Key, sa.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encrypting, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        //Write all data to the stream.
                        sw.Write(keyInput);
                    }

                    encryptedBytes = ms.ToArray();
                }
            }
            return Convert.ToBase64String(encryptedBytes);

        }

        public static string DecryptCommunication(string returnInput, SymmetricAlgorithm symmetricAlgorithm)
        {
            SymmetricAlgorithm sa = symmetricAlgorithm;
            byte[] encryptedBytes = Convert.FromBase64String(returnInput);
            string plaintext;
            ICryptoTransform decrypting = sa.CreateDecryptor(sa.Key, sa.IV);

            using (MemoryStream ms = new MemoryStream(encryptedBytes))
            {
                using (CryptoStream cs = new CryptoStream(ms, decrypting, CryptoStreamMode.Read))
                {
                    using (StreamReader sw = new StreamReader(cs))
                    {
                        //Write all data to the stream.
                        plaintext = sw.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }
    }
}
