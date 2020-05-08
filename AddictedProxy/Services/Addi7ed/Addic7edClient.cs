using System;
using System.Net.Http;

namespace AddictedProxy.Services.Addi7ed
{
    public class Addic7edClient
    {
        private readonly HttpClient _httpClient;

        public Addic7edClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://www.addic7ed.com");
        }
        
    }
}