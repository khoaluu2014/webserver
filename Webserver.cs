using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Tofu.Webserver;

public static class Server
{
    /// <summary>
    /// A lean and mean web server.
    /// </summary>
    private static HttpListener listener;

    /// <summary>
    /// Returns list of IP addresses assigned to localhost network devices,
    /// such as hardwired ethernet, wireless, etc,...
    /// </summary>
    private static List<IPAddress> GetLocalHostIPs()
    {
        IPHostEntry host;
        host = Dns.GetHostEntry(Dns.GetHostName());
        List<IPAddress> ret = host
            .AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
            .ToList();

        return ret;
    }

    private static HttpListener InitializeListener(List<IPAddress> localhostIPs)
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost/");

        localhostIPs.ForEach(ip =>
        {
            Console.WriteLine("Listening on IP " + "http://" + ip.ToString() + "/");
            listener.Prefixes.Add("http://" + ip.ToString() + ":8000/");
        });

        return listener;
    }

    public static int maxSimultaneousConnections = 20;
    private static Semaphore sem = new Semaphore(
        maxSimultaneousConnections,
        maxSimultaneousConnections
    );

    /// <summary>
    /// Begin listening to connections on a separate worker thread
    /// </summary>
    private static void Start(HttpListener listener)
    {
        listener.Start();
        Task.Run(() => RunServer(listener));
    }

    /// <summary>
    /// Start awaiting for connections, up to the "maxSimultaneousConnections value."
    /// This code runs in a separate thread.
    /// </summary>
    private static void RunServer(HttpListener listener)
    {
        while (true)
        {
            sem.WaitOne();
            StartConnectionListener(listener);
        }
    }

    /// <summary>
    /// Await connections.
    /// </summary>
    private static async void StartConnectionListener(HttpListener listener)
    {
        // Wait for a connection. Return to caller while we wait
        HttpListenerContext context = await listener.GetContextAsync();

        // Release the semaphore
        sem.Release();

        // We have a connection, do something
        string response = "Hello Browser!";
        byte[] encoded = Encoding.UTF8.GetBytes(response);
        context.Response.ContentLength64 = encoded.Length;
        context.Response.OutputStream.Write(encoded, 0, encoded.Length);
        context.Response.OutputStream.Close();
    }

    /// </summary>
    /// Starts the web server.
    /// </summary>

    public static void Start()
    {
        List<IPAddress> localHostIPs = GetLocalHostIPs();
        HttpListener listener = InitializeListener(localHostIPs);
        Start(listener);
    }
}
