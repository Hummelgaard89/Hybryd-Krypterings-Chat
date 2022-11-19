using System;
using System.Net.Sockets;
using System.IO;
using Hybrid_Chat_Client;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;
using System.Text;
using System.Linq;
using System.Collections.Generic;

public class EchoClient : TcpClient
{
    public EchoClient(string host)
    {
        base.Connect(host, 7);

    }
    public static void Main(string[] args)
    {
        EchoClient conversant = null;
        StreamWriter sw = null;
        StreamReader sr = null;
        string testing;
        try
        {
            string host = args.Length == 1 ? args[0] : "127.0.0.1";

            conversant = new EchoClient(host);

            NetworkStream ns = conversant.GetStream();

            sw = new StreamWriter(ns);  

            sr = new StreamReader(ns);

            //Encryption_Client ec = new Encryption_Client();


            string input;
            Console.WriteLine("Enter text: 'Exit' to stop normal chat, or 'Encrypt' to start encrypted communication: ");
            //Her skal der ske noget med encrypt
            //Console.WriteLine(ec.RsaPublicKey); 

            while ((input = Console.ReadLine()) != "Exit")
            {
                if (input == "Encrypt")
                {
                    RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
                    string publicKey = csp.ToXmlString(false);
                    string privateKey = csp.ToXmlString(true);
                    sw.WriteLine("Encrypt" + publicKey);
                    ///////
                    string aesKey = null;
                    string aesiv = null;
                    while (aesKey == null || aesiv == null)
                    {
                        csp.FromXmlString(privateKey);
                        string encryptedIncomingText = sr.ReadLine();
                        byte[] encryptedIncomingBytes = Convert.FromBase64String(encryptedIncomingText);
                        byte[] decryptedBytes = csp.Decrypt(encryptedIncomingBytes, true);
                        string decryptedData = Convert.ToBase64String(decryptedBytes);

                        if (decryptedData.StartsWith("AESKEYANDIVKEY") == true)
                        {
                            aesKey = decryptedData.Remove(0, 14);
                        }
                        else if (decryptedData.StartsWith("AESKEYANDIVIV") == true)
                        {
                            aesiv = decryptedData.Remove(0, 13);
                        }
                    }
                    Console.WriteLine("Encryption esablished");
                    while (input != "Exit")
                    {
                        SymmetricAlgorithm aesSession = Aes.Create();
                        aesSession.KeySize = 256;
                        aesSession.Key = Convert.FromBase64String(aesKey);
                        aesSession.IV = Convert.FromBase64String(aesiv);
                        aesSession.Padding = PaddingMode.PKCS7;
                        //string encryptedString = EncryptCommun
                        

                        sw.WriteLine(EncryptCommunication(input, aesSession));
                        string decryptedString = DecryptCommunication(sr.ReadLine(), aesSession);
                        Console.WriteLine("Reply from " + host + ": " + decryptedString);
                        Console.Write("Enter text: 'Exit' to stop encrypted Communication ");

                    }
                    
                    //Console.WriteLine("AES KEY = " + Convert.ToBase64String(aesKey) + "\n" +
                    //                   "AES IV = " + Convert.ToBase64String(aesIV) + "\n");


                    //csp.FromXmlString(publicKey);

                    //byte[] encryptedBytes = csp.Encrypt(bytesToSend.ToArray(), true);
                    //Console.WriteLine("ENCRYPTED ARRAY = " + Convert.ToBase64String(encryptedBytes) + "\n");

                    //Console.WriteLine("\n\n\nNu til dekrypteringen \n\n\n");

                    //csp.FromXmlString(privateKey);

                    //byte[] decryptedBytes = csp.Decrypt(encryptedBytes, true);
                    //string decryptedData = Convert.ToBase64String(decryptedBytes);
                    //Console.WriteLine("PRIVATEKEY = " + privateKey + "\n" +
                    //                  "DECRYPTED ARRAY = " + decryptedBytes + "\n" +
                    //                  "DECRYPTED STRING = " + decryptedData);
                }
                sw.WriteLine(input);
                sw.Flush();
                string returndata = sr.ReadLine();
                Console.WriteLine("Reply from " + host + ": " + returndata);
                Console.Write("Enter text: 'Exit' to stop normal chat, or 'Encrypt' to start encrypted communication: ");
            }

            sw.WriteLine(".");
            sw.Flush();
        }
        catch (Exception e)
        {
            Console.WriteLine(e + " " + e.StackTrace);
        }

        finally
        {
            if (sw != null) sw.Close();
            if(sr != null) sr.Close();
            if(conversant != null) conversant.Close();
        }
    }

    public string GetAESKey(string keyInput)
    {
        string AESKeyClean = keyInput.Remove(0, 14);
        return AESKeyClean;
    }
    public string GetAESIv(string ivInput)
    {
        string AESIVClean = ivInput.Remove(0, 14);
        return AESIVClean;
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