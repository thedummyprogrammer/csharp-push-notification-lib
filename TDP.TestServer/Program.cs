//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

using System;
using System.Threading;
using TDP.PushLib.Server;
using TDP.PushLib.Server.Infrastructure;

using System.Security.Cryptography;

namespace TDP.TestServer
{
    class Program
    {
        private static PushServer<int> _PushServer;
        private static ManualResetEvent _ResetEvent;
        private static bool _InitOk;

        static void Main(string[] args)
        {
            _ResetEvent = new ManualResetEvent(false);

            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine(" THE DUMMY PROGRAMMER - TEST PUSH NOTIFICATIONS SERVER ");
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine();

            _PushServer = new PushServer<int>();
            _PushServer.InitStarted += _PushServer_InitStarted;
            _PushServer.InitEnded += _PushServer_InitEnded;
            _PushServer.ShutdownStarted += _PushServer_ShutdownStarted;
            _PushServer.ShutdownEnded += _PushServer_ShutdownEnded;

            _PushServer.StartAsync();

            _ResetEvent.WaitOne();

            if (!_InitOk)
            {
                Console.WriteLine("Press a key to exit...");
                Console.ReadKey();
                return;
            }

            while (true)
            {
                Console.WriteLine($"Enter the client number (0-{int.MaxValue}) you want to send a notification and the message separated by comma or 'EXIT' to close this program");
                string Command = Console.ReadLine();
                
                if (Command == "EXIT")
                    break;

                string[] CommandParts = Command.Split(',');

                if (CommandParts.Length < 2)
                {
                    Console.WriteLine("Invalid command");
                    continue;
                }
                
                if (!System.Text.RegularExpressions.Regex.Match(CommandParts[0], @"\d{1,4}").Success)
                {
                    Console.WriteLine("Invalid client number");
                    continue;
                }

                int ClientID = int.Parse(CommandParts[0]);
                string Message = CommandParts[1];
                _PushServer.SendPushMessage(ClientID, Message);
            }

            _PushServer.Stop();
            _ResetEvent.WaitOne();

            Console.WriteLine("Press a key to close...");
            Console.ReadKey();
        }

        private static void _PushServer_InitStarted(object sender, EventArgs e)
        {
            Console.WriteLine("Initializing server...");
        }

        private static void _PushServer_InitEnded(object sender, InitEndedEventArgs e)
        {
            if (e.Result.Success)
            {
                Console.WriteLine("Initialization completed");
                _InitOk = true;
            }
            else
                Console.WriteLine("Initialization failed");

            Console.WriteLine();
            Console.WriteLine();
            _ResetEvent.Set();
        }

        private static void _PushServer_ShutdownStarted(object sender, EventArgs e)
        {
            Console.WriteLine("Shutting down server...");
        }

        private static void _PushServer_ShutdownEnded(object sender, EventArgs e)
        {
            Console.WriteLine("Shutting down completed.");
            _ResetEvent.Set();
        }
    }
}
