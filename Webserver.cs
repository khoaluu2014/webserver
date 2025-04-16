using System.Net;
using System.Net.Sockets;

namespace Tofu.Webserver
{
public static class Server
{
    /// <summary>
    /// A lean and mean web server.
    /// </summary>
    private static HttpListener listener;
    private static List<IPAddress> GetLocalHostIPs()
    {
        IPHostEntry host;
        host = Dns.GetHostEntry(Dns.GetHostName());
        List<IPAddress> ret = host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToList();
    }

    private static HttpListener InitializeListener(List<IPAddress> localhostIPs)
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost/");

        localhostIPs.ForEach(ip =>
        {
            Console.WriteLine("Listening on IP" + "http://" + ip.ToString() + "/");
            listener.Prefixes.Add("http://" + ip.ToString() + "/");
        });

        return listener;
    }
}
}
