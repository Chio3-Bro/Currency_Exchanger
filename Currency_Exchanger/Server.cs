using BackEnd.Services;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BackEnd
{
    class Server
    {
        static int activeConnections = 0;
        static int MAX_CONNECTIONS = 100;

        static readonly object _lock = new();

        static void Main(string[] args)
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(ipAddress, 8888);
            listener.Start();
            Console.WriteLine("[System]: Server started successfully on port 8888!");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                
                lock (_lock)
                {
                    if (activeConnections >= MAX_CONNECTIONS)
                    {
                        client.Close();
                        continue;
                    }

                    activeConnections++;
                }

                Console.WriteLine("[System]: Client connected!");
                Task.Run(() => HandleClient(client));
            }
        }

        static void HandleClient(TcpClient client)
        {
            AuthService authService = new AuthService();
            CurrencyService currencyService = new CurrencyService();
            RateLimiter rateLimiter = new RateLimiter();

            bool authenticated = false;
            string username = "unknown";

            try
            {
                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

                writer.WriteLine("[Server]: Welcome!");

                while (true)
                {
                    string? message = reader.ReadLine();

                    if (message == null)
                        break;

                    // AUTHENTICATION
                    if (!authenticated)
                    {
                        if (message.StartsWith("/login") || message.StartsWith("/l"))
                        {
                            var parts = message.Split(' ');

                            if (parts.Length != 3)
                            {
                                writer.WriteLine("[Server]: Invalid login command. Use: /login <username> <password>");
                            }

                            if (!authService.Login(parts[1], parts[2]))
                            {
                                writer.WriteLine("[Server]: Invalid username or password.");
                            }

                            authenticated = true;
                            username = parts[1];

                            Logger.Log($"{username} logged in.");
                            writer.WriteLine("[Server]: Login successful!");
                            continue;
                        }
                        else
                        {
                            writer.WriteLine("[Server]: Please login first using /login <username> <password>");
                        }
                    }

                    // EXIT
                    if (message == "/exit")
                    {
                        Console.WriteLine("[System]: Client disconnected!");
                        writer.WriteLine("BYE");
                        break;
                    }

                    // RATE LIMITER
                    if (!rateLimiter.Allow())
                    {
                        writer.WriteLine("[Server]: Too many requests. Please wait.");
                        break;
                    }

                    // CURRENCY EXCHANGE
                    var data = message.Split(' ');

                    if (data.Length != 2)
                    {
                        writer.WriteLine("[Server]: Invalid command. Use: <FROM_CURRENCY> <TO_CURRENCY>");
                        continue;
                    }

                    string fromCurrency = data[0].ToUpper();
                    string toCurrency = data[1].ToUpper();

                    decimal? rate = currencyService.GetRate(fromCurrency, toCurrency);

                    if (rate == null)
                        writer.WriteLine("[Server]: Unknown currency.");
                    else
                    {
                        writer.WriteLine($"[Server]: RATE {rate}");
                        Logger.Log($"{username} requested {fromCurrency} {toCurrency}");
                    }
                }
            }
            finally
            {
                lock (_lock)
                {
                    activeConnections--;
                }

                Logger.Log($"{username} disconnected.");
                client.Close();
            }
        }
    }
}
