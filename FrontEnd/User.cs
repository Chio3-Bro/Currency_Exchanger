using System.Net.Sockets;

namespace FrontEnd
{
    class User
    {
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient("127.0.0.1", 8888);

            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
            using StreamWriter writer = new StreamWriter(stream, System.Text.Encoding.UTF8) { AutoFlush = true };

            Console.WriteLine(reader.ReadLine());

            while (true)
            {
                string input = Console.ReadLine();
                writer.WriteLine(input);

                string response = reader.ReadLine();
                Console.WriteLine($"[Server]: {response}");

                if (response == "BYE") break;
            }

            client.Close();
        }
    }
}
