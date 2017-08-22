using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin;
using System.Threading;
using Microsoft.AspNet.SignalR.Hubs;
using ZeroMQ;
using System.Collections.Generic;
using Newtonsoft.Json;
using Webscoket.Models;
using System.Linq;

[assembly: OwinStartup(typeof(WebScoket.Startup))]
namespace WebScoket
{

    class Program
    {
        static void Main(string[] args)
        {
            // This will *ONLY* bind to localhost, if you want to bind to all addresses
            // use http://*:8080 to bind to all addresses. 
            // See http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx 
            // for more information.
            string url = "http://10.38.129.14:8081";
            using (WebApp.Start(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.ReadLine();
            }
        }
    }
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
            app.MapSignalR("/signalr", new HubConfiguration());

        }

        [HubName("MyHub")]
        public class MyHub : Hub
        {
            public void Send()
            {
                Console.WriteLine("MyHub connected to server");
                System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
                string topic = "device";
                string url = "tcp://10.38.129.170:5560";
                using (var context = new ZContext())
                using (var subscriber = new ZSocket(context, ZSocketType.SUB))
                {
                    //Create the Subscriber Connection
                    subscriber.Connect("tcp://10.38.129.170:5560");
                    Console.WriteLine("Subscriber started for Topic with URL : {0} {1}", topic, url);
                    subscriber.Subscribe(topic);
                    int subscribed = 0;
                    List<LocationData> lstLocationData = new List<LocationData>();

                    while (true)
                    {

                        using (ZMessage message = subscriber.ReceiveMessage())
                        {
                            subscribed++;
                            
                            string contents = message[1].ReadString();
                            Console.WriteLine(contents);

                            LocationData objLocationData = JsonConvert.DeserializeObject<ListOfArea>(contents).device_notification.records.FirstOrDefault();

                            //Console.WriteLine(lstLocationData.Count());
                            //Console.WriteLine(objLocationData.mac);
                            //Console.WriteLine(lstLocationData.Any(m => m.mac == objLocationData.mac));

                            if(lstLocationData.Any(m=>m.mac==objLocationData.mac))
                            {
                                Console.WriteLine("Enter into the section to Update old macAddress");
                                //Update the particular MacAddress from the List of data
                                var LocationDataObject=lstLocationData.FirstOrDefault(m => m.mac == objLocationData.mac);
                                LocationDataObject.x = objLocationData.x;
                                LocationDataObject.y = objLocationData.y;
                               //If old MacAddress then just update the x and y axis of the column
                            }
                            else
                            {
                                Console.WriteLine("Enter into the section to Add new macAddress");
                                //If any new MacAddress present then add the particular in the list
                                lstLocationData.Add(objLocationData);
                            }
                           
                            string strReturn = JsonConvert.SerializeObject(lstLocationData);
                            Clients.All.addMessage(strReturn);
                            Console.Write("Added successfully to server");
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
        }
    }
}