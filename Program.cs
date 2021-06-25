using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

public static class Program {
    private const int BaseBufferSize = 8192;
    private const int Iterations = 1024 * 32;
    private static readonly byte[] StatusLine = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n\r\n");
    
    private static long Timestamp() {
        return DateTime.UtcNow.Ticks / 10000;
    }
    
    public static void Main(string[] arguments) {
        var bufferSizeShift = arguments.Length > 0 ? Math.Max(0, int.Parse(arguments[0])) : 6;
        var bufferSize = BaseBufferSize << bufferSizeShift;
        Console.WriteLine($"Buffer size: {bufferSize} (shift: {bufferSizeShift})");
        Console.WriteLine($"Iterations: {Iterations}");
        
        var sourceLocation = Path.GetTempFileName();
        
        using (var source = File.Open(sourceLocation, FileMode.Open)) {
            source.Write(new byte[bufferSize], 0, bufferSize);
            
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) {
                var address = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 48081);
                listener.Bind(address);
                listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                listener.Listen(1);
                
                Console.WriteLine("Listening for incoming connections");
                
                var readBuffer = new byte[bufferSize];
                
                while (true) {
                    var client = listener.Accept();
                    Console.WriteLine("Accepted client connection");
                    client.Receive(new byte[BaseBufferSize]);
                    
                    client.Send(StatusLine);
                    
                    var snapshot = Timestamp();
                    for (int i = 0; i < Iterations; i++) {
                        source.Seek(0, SeekOrigin.Begin);
                        source.Read(readBuffer, 0, bufferSize);
                        client.Send(readBuffer);
                    }
                    Console.WriteLine($"File proxying took {Timestamp() - snapshot} milliseconds");
                        
                    Console.WriteLine("Done serving client connection");
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
            }
        }
    }
} 
