// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PartsUnlimited.Security
{
    internal static class AppBuilderLoginProviderExtensions
    {
        public static IConfigurationRoot Configuration { get; }

        public static void AddLoginProviders(this IServiceCollection services, ILoginProviders loginProviders)
        {
            if (loginProviders.Azure.Use)
            {
                services.AddAuthentication(options => {
                                            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                                            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                                        })
                        .AddCookie()
                        .AddOpenIdConnect(options => {
                            options.Authority = Configuration["auth:oidc:authority"];
                            options.ClientId = Configuration["auth:oidc:clientid"];
                        });
            }

            if (loginProviders.Facebook.Use)
            {
                services.AddAuthentication()
                        .AddFacebook(options => {
                            options.AppId = Configuration["auth:facebook:appid"];
                            options.AppSecret = Configuration["auth:facebook:appsecret"];
                        });
            }

            if (loginProviders.Google.Use)
            {
                services.AddAuthentication()
                        .AddGoogle(options => {
                            options.ClientId = Configuration["auth:google:clientid"];
                            options.ClientSecret = Configuration["auth:google:clientsecret"];
                        });
            }

            if (loginProviders.Twitter.Use)
            {
                services.AddAuthentication()
                        .AddTwitter(options => {
                            options.ConsumerKey = Configuration["auth:twitter:consumerkey"];
                            options.ConsumerSecret = Configuration["auth:twitter:consumersecret"];
                        });
            }

            if (loginProviders.Microsoft.Use)
            {
                //The MicrosoftAccount service has restrictions that prevent the use of http://localhost:5001/ for test applications.
                //As such, here is how to change this sample to uses http://ktesting.com:5001/ instead.

                //Edit the Project.json file and replace http://localhost:5001/ with http://ktesting.com:5001/.

                //From an admin command console first enter:
                // notepad C:\Windows\System32\drivers\etc\hosts
                //and add this to the file, save, and exit (and reboot?):
                // 127.0.0.1 ktesting.com

                //Then you can choose to run the app as admin (see below) or add the following ACL as admin:
                // netsh http add urlacl url=http://ktesting:12345/ user=[domain\user]

                //The sample app can then be run via:
                // k web

                services.AddAuthentication()
                        .AddMicrosoftAccount(options => {
                            options.ClientId = Configuration["auth:microsoft:clientid"];
                            options.ClientSecret = Configuration["auth:microsoft:clientsecret"];
                        });
            }
        }
    }
}