using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DuckDuckGo.Net
{
    /// <summary>
    /// IApiClient Interface
    /// </summary>
    public interface IApiClient
    {
        string Load(string uri);

        Task<string> LoadAsync(string uri);
    }
}
