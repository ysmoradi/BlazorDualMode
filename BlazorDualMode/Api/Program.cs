﻿using Bit.Owin;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace BlazorDualMode.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await BuildWebHost(args)
                .RunAsync();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            BitWebHost.CreateDefaultBuilder(args)
                .UseStartup<AppStartup>()
                .Build();
    }
}
