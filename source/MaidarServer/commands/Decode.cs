using System;
using MaiDar.Core;

namespace MaiDarServer.commands
{
    internal class Decode
    {
        public const string CommandName = "decode";

        public static void Start(string val)
        {
            Console.WriteLine("解密字符串: {0}", TranslateUtils.DecryptStringBySecretKey(val));
        }
    }
}
