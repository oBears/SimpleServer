using System;
using System.IO;
using System.Text;

public class HttpResponse
{

    public HttpResponse(int code, string contentType, Encoding encoding, byte[] content)
    {
        StatusCode = code;
        ContentType = contentType;
        Encoding = encoding;
        Content = content;
    }
    public HttpResponse(int code, Encoding encoding, string content)
    {
        StatusCode = code;
        ContentType = "text/html";
        Encoding = encoding;
        Content = encoding.GetBytes(content);
    }
    public int StatusCode { set; get; }
    public string ContentType { set; get; }
    public Encoding Encoding { set; get; }
    public byte[] Content { set; get; }
    public byte[] GetResponse()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"HTTP/1.1 {StatusCode}");
        sb.AppendLine($"Content-Type:{ContentType}");
        sb.AppendLine("");
        var headerBytes = Encoding.GetBytes(sb.ToString());
        var allBytes = new byte[headerBytes.Length + Content.Length];
        headerBytes.CopyTo(allBytes, 0);
        Content.CopyTo(allBytes, headerBytes.Length);
        return allBytes;
    }
}