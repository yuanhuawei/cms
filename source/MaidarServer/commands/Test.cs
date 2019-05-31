using System;
using System.Threading;
using System.Threading.Tasks;

namespace MaiDarServer.commands
{
    internal class Test
    {
        public const string CommandName = "test";

        public static void Start(bool isAll)
        {
            Console.WriteLine(isAll);
        }
    }
}
