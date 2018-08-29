using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAccountLocker.Model
{
    [Serializable]
    public class KeyFile
    {
        public byte[] key;
        public byte[] IV;
        public KeyFile()
        {
            Generate();
        }
        public KeyFile(bool generate)
        {
            if (generate)
                Generate();
        }
        private void Generate()
        {
            AesManaged aes = new AesManaged();
            aes.KeySize = 256;
            aes.GenerateKey();
            aes.GenerateIV();
            key = aes.Key;
            IV = aes.IV;
        }
    }
}
