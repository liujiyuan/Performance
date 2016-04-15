// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Stress.Framework;
using Xunit;

namespace Microsoft.AspNetCore.Tests.Stress
{
    public class StarterMvcTests : StressTestBase
    {
        public static async Task StarterMvc_Warmup(HttpClient client)
        {
            await client.GetAsync("/");
            await client.GetAsync("/Home/About");
            await client.GetAsync("/Home/Contact");
            await client.GetAsync("/Account/Login");
            await client.GetAsync("/Account/LogOff");
            await client.GetAsync("/Account/Register");
            await client.GetAsync("/Manage");
        }

        //[Stress("StarterMvc", WarmupMethodName = nameof(StarterMvc_Warmup))]
        public void StarterMvc()
        {
            IterateAsync(client =>
            {
                var response = client.GetAsync("/").GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
                response = client.GetAsync("/Home/About").GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
                response = client.GetAsync("/Home/Contact").GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                // Register
                var getResponse = client.GetAsync("/Account/Register").GetAwaiter().GetResult();
                getResponse.EnsureSuccessStatusCode();

                var responseContent = getResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var verificationToken = ExtractVerificationToken(responseContent);

                var testUser = GetUniqueUserId();
                var requestContent = CreateRegisterPost(verificationToken, testUser, "Asd!123$$", "Asd!123$$");

                var postResponse = client.PostAsync("/Account/Register", requestContent).GetAwaiter().GetResult();
                postResponse.EnsureSuccessStatusCode();

                var postResponseContent = postResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Assert.Contains("Learn how to build ASP.NET apps that can run anywhere.", postResponseContent); // Home page

                // Verify manage page
                var manageResponse = client.GetAsync("/Manage").GetAwaiter().GetResult();
                manageResponse.EnsureSuccessStatusCode();

                var manageContent = manageResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                verificationToken = ExtractVerificationToken(manageContent);

                // Verify Logoff
                var logoffRequestContent = CreateLogOffPost(verificationToken);
                var logoffResponse = client.PostAsync("/Account/LogOff", logoffRequestContent).GetAwaiter().GetResult();
                logoffResponse.EnsureSuccessStatusCode();

                var logOffResponseContent = logoffResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Assert.Contains("Learn how to build ASP.NET apps that can run anywhere.", postResponseContent); // Home page

                // Verify relogin
                var loginResponse = client.GetAsync("/Account/Login").GetAwaiter().GetResult();
                loginResponse.EnsureSuccessStatusCode();
                var loginResponseContent = loginResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                verificationToken = ExtractVerificationToken(responseContent);
                var loginRequestContent = CreateLoginPost(verificationToken, testUser, "Asd!123$$");

                var loginPostResponse = client.PostAsync("/Account/Login", loginRequestContent).GetAwaiter().GetResult();
                loginPostResponse.EnsureSuccessStatusCode();

                var longPostResponseContent = loginPostResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Assert.DoesNotContain("Invalid login attempt.", longPostResponseContent); // Errored Login page

                // Logoff to get the HttpClient back into a working state.
                manageResponse = client.GetAsync("/Manage").GetAwaiter().GetResult();
                manageResponse.EnsureSuccessStatusCode();
                manageContent = manageResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                verificationToken = ExtractVerificationToken(manageContent);
                logoffRequestContent = CreateLogOffPost(verificationToken);
                logoffResponse = client.PostAsync("/Account/LogOff", logoffRequestContent).GetAwaiter().GetResult();
                logoffResponse.EnsureSuccessStatusCode();
            });
        }

        private HttpContent CreateRegisterPost(
            string verificationToken,
            string email,
            string password,
            string confirmPassword)
        {
            List<KeyValuePair<string, string>> form = new List<KeyValuePair<string, string>>();

            form.Add(new KeyValuePair<string, string>("__RequestVerificationToken", verificationToken));
            form.Add(new KeyValuePair<string, string>("Email", email));
            form.Add(new KeyValuePair<string, string>("Password", password));
            form.Add(new KeyValuePair<string, string>("ConfirmPassword", confirmPassword));

            var content = new FormUrlEncodedContent(form);

            return content;
        }

        private HttpContent CreateLoginPost(string verificationToken, string email, string password)
        {
            List<KeyValuePair<string, string>> form = new List<KeyValuePair<string, string>>();

            form.Add(new KeyValuePair<string, string>("__RequestVerificationToken", verificationToken));
            form.Add(new KeyValuePair<string, string>("Email", email));
            form.Add(new KeyValuePair<string, string>("Password", password));
            form.Add(new KeyValuePair<string, string>("RememberMe", false.ToString()));

            var content = new FormUrlEncodedContent(form);

            return content;
        }

        private HttpContent CreateLogOffPost(string verificationToken)
        {
            List<KeyValuePair<string, string>> form = new List<KeyValuePair<string, string>>();

            form.Add(new KeyValuePair<string, string>("__RequestVerificationToken", verificationToken));

            var content = new FormUrlEncodedContent(form);

            return content;
        }

        private string ExtractVerificationToken(string response)
        {
            string tokenElement = string.Empty;
            var writer = new StreamWriter(new MemoryStream());
            writer.Write(response);

            writer.BaseStream.Position = 0;
            var reader = new StreamReader(writer.BaseStream);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine().Trim();
                if (line.StartsWith("<input name=\"__RequestVerificationToken\""))
                {
                    tokenElement = line.Replace("</form>", "");
                }
            }

            XElement root = XElement.Parse(tokenElement);
            return (string)root.Attribute("value");
        }

        private string GetUniqueUserId()
        {
            return string.Format("testUser{0}@ms.com", Guid.NewGuid().ToString());
        }
    }
}
