//*****************************************************************************
//
//  By The Dummy Programmer
//  https://www.thedummyprogrammer.com
//
//*****************************************************************************

using System;
using System.Threading;
using TDP.PushLib.Client;
using TDP.PushLib.Messages;
using TDP.PushLib.Client.Infrastructure;

namespace TDP.TestClient
{
    class Program
    {
        private static PushClient<int> _PushClient;
        private static ManualResetEvent _ResetEvent;
        private static bool _RegistrationOk;

        static void Main(string[] args)
        {
            _ResetEvent = new ManualResetEvent(false);

            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine(" THE DUMMY PROGRAMMER - TEST PUSH NOTIFICATIONS CLIENT ");
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine($"Insert your client ID (How do you identify yourself, 0-{int.MaxValue})");

            bool IsValidClientID = false;
            string ClientID = string.Empty;

            while (!IsValidClientID)
            {
                ClientID = Console.ReadLine();
                IsValidClientID = System.Text.RegularExpressions.Regex.Match(ClientID, @"\d{1,4}").Success;
                if (!IsValidClientID)
                    Console.WriteLine("Invalid client number");
            }

            AuthenticationToken<int> AuthToken = new AuthenticationToken<int>(int.Parse(ClientID), DateTime.Now, DateTime.Now.AddHours(24), null);
            Console.WriteLine("Starting push notification client...");
            _PushClient = new PushClient<int>();
            _PushClient.RegisterClientStarted += _PushClient_RegisterClientStarted;
            _PushClient.RegisterClientEnded += _PushClient_RegisterClientEnded;
            _PushClient.PushMessageReceived += _PushClient_PushMessageReceived;
            _PushClient.ConnectionClosed += _PushClient_ConnectionClosed;
            _PushClient.ConnectAsync(AuthToken);

            _ResetEvent.WaitOne();

            if (!_RegistrationOk)
            {
                Console.WriteLine("Press a key to exit...");
                Console.ReadKey();
                return;
            }

            while (true)
            {
                Console.WriteLine("Waiting for some push message... type EXIT to close this program");
                string Command = Console.ReadLine();

                if (Command == "EXIT")
                    break;
            }
        }

        private static void _PushClient_RegisterClientStarted(object sender, EventArgs e)
        {
            Console.WriteLine("Registering the client...");
        }

        private static void _PushClient_RegisterClientEnded(object sender, RegisterClientEndedEventArgs<int> e)
        {
            if (e.RegisterClientResponse.Success)
            {
                Console.WriteLine("Registration completed");
                _RegistrationOk = true;
            }
            else
            {
                Console.WriteLine("Registration failed");
            }

            Console.WriteLine();
            Console.WriteLine();
            _ResetEvent.Set();
        }

        private static void _PushClient_PushMessageReceived(object sender, PushMessageReceivedEventArgs e)
        {
            Console.WriteLine("Received message: " + e.PushMessage.Message);
        }

        private static void _PushClient_ConnectionClosed(object sender, EventArgs e)
        {
            Console.WriteLine("Connection closed, closing application");
            System.Environment.Exit(0);
        }
        

    }
}
