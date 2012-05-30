using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using RealWorld.RestClient.Models;

namespace RealWorld.RestClient
{
    internal class Program
    {
        private static void Main()
        {
            /*
            * 
            * Client side batch process to interact with API
            * 
            * This code mimics some automated process that
            * invokes the API.
            * 
            */
            try
            {
                Prompt("Start rest process>>");

                var apiClient = new HttpClient();
                //var url = "http://localhost:36134/rest/loginWithLinks";
                var url = "http://phillysnug.apphb.com/rest/loginWithLinks";

                var responseTask = GetResponseTask(apiClient, url, "GET", null);
                responseTask.Wait();

                //Get the links for the service:
                var startResults = DeserializeJsonAs<Credentials>(responseTask);
                var links = startResults.Links.ToDictionary(item => item.Rel);

                //Login to API
                var signinLink = links["SignIn"];
                responseTask = GetResponseTask(
                    apiClient,
                    signinLink.Href,
                    signinLink.Method,
                    SetupClientCredentials());

                responseTask.Wait();

                var responseCredentials = DeserializeJsonAs<Credentials>(responseTask);

                if (responseCredentials.Password.StartsWith("IsSignedIn"))
                {
                    //Signed in successfully. Trigger SomeProcess:
                    links = responseCredentials.Links.ToDictionary(item => item.Rel);
                    var triggerSomeProcessLink = links["TriggerSomeProcess"];
                    responseTask = GetResponseTask(
                        apiClient,
                        triggerSomeProcessLink.Href,
                        triggerSomeProcessLink.Method,
                        null);

                    responseTask.Wait();

                    //Do something based on the status of SomeProcess:
                    var someProcessResult = DeserializeJsonAs<BizProcessStatus>(responseTask);
                    Console.WriteLine("\n\rStatus: " + someProcessResult.Status);

                    foreach (var detail in someProcessResult.ProcessingDetails)
                    {
                        Console.WriteLine(string.Format("  {0}: {1}", detail.Description, detail.Href));
                    }
                }

                //TODO: add logic to handle when automated process wasn't able to sign in
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            Prompt("Exit rest process>>");
        }

        private static Task<HttpResponseMessage> GetResponseTask(
            HttpClient client,
            string url,
            string method,
            ObjectContent content)
        {
            switch (method.ToUpper())
            {
                case "GET":
                    return client.GetAsync(url);

                case "POST":
                    return client.PostAsync(url, content);

                case "PUT":
                    return client.PutAsync(url, content);

                case "DELETE":
                    return client.DeleteAsync(url);

                default:
                    throw new InvalidOperationException("Invalid HTTP method");
            }
        }

        private static T DeserializeJsonAs<T>(Task<HttpResponseMessage> responseTask)
        {
            var response = responseTask.Result;
            response.EnsureSuccessStatusCode();

            var readtask = response.Content.ReadAsAsync<T>();
            readtask.Wait();

            return readtask.Result;
        }

        public static ObjectContent CreateJsonObjectContent<T>(T model)
        {
            var requestMessage = new HttpRequestMessage();
            return requestMessage.CreateContent(
                model,
                MediaTypeHeaderValue.Parse("application/json"),
                new MediaTypeFormatter[] { new JsonMediaTypeFormatter() },
                new FormatterSelector());
        }

        private static ObjectContent SetupClientCredentials()
        {
            var credentials = new Credentials { UserName = "AutomatedUser", Password = "SomeSuperSecretPassword" };

            return CreateJsonObjectContent(credentials);

        }

        private static void Prompt(string prompt)
        {
            Console.Write("\n\r" + prompt);
            Console.ReadLine();
        }

    }
}