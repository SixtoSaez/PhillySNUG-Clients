using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using RealWorld.ApiClient.Models;

namespace RealWorld.ApiClient
{
    class Program
    {

        static void Main()
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
                Prompt("Start process>>");

                var apiClient = new HttpClient();
                var url = "http://phillysnug.apphb.com/api/login";

                //Login to API
                var responseTask = apiClient.PostAsync(url, SetupClientCredentials());
                responseTask.Wait();

                var responseCredentials = DeserializeJsonAs<Credentials>(responseTask);

                if (responseCredentials.Password.StartsWith("IsSignedIn"))
                {
                    //Signed in successfully. Trigger SomeProcess:
                    url = "http://phillysnug.apphb.com/api/someprocess";
                    responseTask = apiClient.GetAsync(url);
                    responseTask.Wait();

                    //Do something based on the status of SomeProcess:
                    var someProcessResult = DeserializeJsonAs<BizProcessStatus>(responseTask);
                    Console.WriteLine("\n\rStatus: " + someProcessResult.Status);

                    foreach (var detail in someProcessResult.ProcessingDetails)
                    {
                        Console.WriteLine("   Processing detail: " + detail);
                    }
                }

                //TODO: add logic to handle when automated process wasn't able to sign in
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            Prompt("Exit process>>");
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
