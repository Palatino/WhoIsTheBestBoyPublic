using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestBoyClient.Extensions
{
    public static class JsRuntimeExtensions
    {
        public static async Task<IJSObjectReference> CreateCroppie(this IJSRuntime jsRuntime, string urlLink, string extension)
        {
            extension = extension.Replace(".","" );
            return await jsRuntime.InvokeAsync<IJSObjectReference>("startCroppie", urlLink, extension);
        }
    }
}
