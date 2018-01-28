using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

public class HttpRequest
{
    public HttpRequest(string txt)
    {
        var arys = txt.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var row0 = Regex.Split(arys[0], @"(\s+)")
        .Where(x => !string.IsNullOrEmpty(x.Trim()))
        .ToList();
        Method = row0[0];
        Url = row0[1];
    }
    public string Url { set; get; }
    public string Method { set; get; }

}