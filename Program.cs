using Fleck;
using System.Runtime.InteropServices;
public class Program
{
    [DllImport("user32.dll")]
    static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
    public const byte PlayPause = 0xB3;
    public const byte Next = 0xB0;
    public const byte Prev = 0xB1;
    private static Dictionary<string, byte> _commands = new Dictionary<string, byte>() {
        {"playpause", 0xB3},
        {"next", 0xB0},
        {"prev", 0xB1}
    };

    public static void ConsoleBehavior()
    {
        var key = Console.ReadKey();
        if (key.KeyChar == 'm')
        {
            keybd_event(PlayPause, 0, 0, UIntPtr.Zero);
            ConsoleBehavior();
        }
        else if (key.KeyChar == 'p')
        {
            return;
        }
        else
        {
            ConsoleBehavior();
        }
    }
    public static void Main(string[] args)
    {
        int portArg = -1;
        Int32.TryParse(args.Length > 0 ? args[0] : "", out portArg);
        portArg = portArg > 0 ? portArg : 9070;
        var server = new WebSocketServer($"ws://127.0.0.1:{portArg}");
        server.Start(socket =>
        {
            socket.OnOpen = () => {
                socket.Send("Connected to media control!");
            };
            socket.OnMessage = msg => {
                if (msg == "close")
                {
                    socket.Close();
                }
                if (_commands.TryGetValue(msg, out byte key))
                {
                    keybd_event(key, 0, 0, UIntPtr.Zero);
                }
            };
        });
        ConsoleBehavior();
    }
}