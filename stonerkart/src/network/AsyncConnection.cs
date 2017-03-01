using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace stonerkart
{
    abstract class AsyncConnection
    {
        protected Socket socket;
        private byte[] buffer = new byte[1024];
        private StringBuilder builder;

        public AsyncConnection(Socket socket)
        {
            this.socket = socket;
            fixKey();
            builder = new StringBuilder();
            xd();
        }

        public void send(Message m)
        {
            socket.Send(encrypt(m.getBytes()));
        }

        protected abstract void handle(Message m);

        protected abstract void closed();

        private void fixKey()
        {

            ECDiffieHellmanCng myStuff = new ECDiffieHellmanCng(256);
            myStuff.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            myStuff.HashAlgorithm = CngAlgorithm.Sha256;
            
            byte[] myBytes = myStuff.PublicKey.ToByteArray();
            socket.Send(myBytes);
            byte[] theirBytes = new byte[72];
            socket.Receive(theirBytes);



            ECDiffieHellmanPublicKey tb = ECDiffieHellmanCngPublicKey.FromByteArray(theirBytes, CngKeyBlobFormat.GenericPublicBlob);
            byte[] sharedSecret = myStuff.DeriveKeyMaterial(tb);
            byte[] salt = hashMe(sharedSecret.Take(16).ToArray());

            Rijndael rijndael = Rijndael.Create();
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(sharedSecret, salt, 420);
            rijndael.Key = pdb.GetBytes(32);
            rijndael.IV = pdb.GetBytes(16);

            encryptor = rijndael.CreateEncryptor();
            decryptor = rijndael.CreateDecryptor();
        }


        private byte[] hashMe(byte[] bs)
        {
            SHA384Cng sha = new SHA384Cng();
            return sha.ComputeHash(bs);
        }

        private ICryptoTransform encryptor;
        private ICryptoTransform decryptor;


        private byte[] encrypt(byte[] bs)
        {
            MemoryStream memoryStream;
            CryptoStream cryptoStream;
            memoryStream = new MemoryStream();
            cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(bs, 0, bs.Length);
            cryptoStream.Close();
            return memoryStream.ToArray();
        }

        private byte[] decrypt(byte[] bs)
        {
            MemoryStream memoryStream;
            CryptoStream cryptoStream;
            memoryStream = new MemoryStream();
            cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write);
            cryptoStream.Write(bs, 0, bs.Length);
            cryptoStream.Close();
            return memoryStream.ToArray();
        }
        
        private byte[] decrypt(byte[] bs, int c)
        {
            return decrypt(bs.Take(c).ToArray());
        }

        private void xd()
        {
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, rc, buffer);
        }

        private void rc(IAsyncResult r)
        {
            int read;
            try
            {
                read = socket.EndReceive(r);
            }
            catch (Exception)
            {
                closed();
                return;
            }


            builder.Append(Encoding.ASCII.GetString(decrypt(buffer, read)));

            if (read < buffer.Length)
            {
                Message m = new Message(builder.ToString());
                builder = new StringBuilder();
                handle(m);
            }

            xd();
        }
    }
}