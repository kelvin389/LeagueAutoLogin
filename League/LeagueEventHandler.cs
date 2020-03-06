/*
 * Most of this code is taken from here:
 * https://github.com/BryanHitchcock/lcu-sharp
 */

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using WebSocketSharp;

namespace League
{
    internal class LeagueEventHandler
    {
        private static readonly Dictionary<string, List<EventHandler<LeagueEvent>>> _subscribers = new Dictionary<string, List<EventHandler<LeagueEvent>>>();

        public static EventHandler<LeagueEvent> MessageReceived { get; set; }

        private static WebSocket _webSocket;
        private static bool subscribed = false;

        public static void Connect()
        {
            // intialize socket to connect to LCU
            Init();

            while (!_webSocket.IsAlive)
            {
                // connect to LCU socket
                _webSocket.Connect();

                Thread.Sleep(1000);
            }

            // 5 subscribes to event, OnJsonApiEvent is all events
            _webSocket.Send("[5, \"OnJsonApiEvent\"]");
            subscribed = true;
        }

        public static void UnsubscribeSocket()
        {
            // unsubscribe from all events
            if (subscribed) _webSocket.Send("[6, \"OnJsonApiEvent\"]");
        }

        private static void Init()
        {
            int port = API.client.GetHttpClient().BaseAddress.Port;
            string tokenEncoded = API.client.GetHttpClient().DefaultRequestHeaders.Authorization.Parameter; // token encoded in base64
            byte[] tokenBytes = Convert.FromBase64String(tokenEncoded); // byte array representation of the encoded string
            string token = Encoding.UTF8.GetString(tokenBytes); // convert byte array to utf8 string
            token = token.Substring(token.IndexOf(":") + 1); // remove "riot:" from string

            // set socket settings
            _webSocket = new WebSocket($"wss://127.0.0.1:{port}/", "wamp");
            _webSocket.SetCredentials("riot", token, true);
            _webSocket.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;
            _webSocket.SslConfiguration.ServerCertificateValidationCallback = (response, cert, chain, errors) => true;

            // subscribe to errors and responses
            _webSocket.OnError += OnError;
            _webSocket.OnMessage += OnMessage;
        }

        private static void OnMessage(object sender, MessageEventArgs e)
        {
            // Check if the message is json received from the client
            if (e.IsText)
            {
                var eventArray = JArray.Parse(e.Data);
                var eventNumber = eventArray[0].ToObject<int>();

                if (eventNumber == 8)
                {
                    var leagueEvent = eventArray[2].ToObject<LeagueEvent>();
                    MessageReceived?.Invoke(sender, leagueEvent);

                    // Call subscribers
                    if (_subscribers.TryGetValue(leagueEvent.Uri, out List<EventHandler<LeagueEvent>> eventHandlers))
                    {
                        foreach (var eventHandler in eventHandlers)
                            eventHandler?.Invoke(sender, leagueEvent);
                    }
                }
            }
        }

        private static void OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        public static void Subscribe(string uri, EventHandler<LeagueEvent> eventHandler)
        {
            if (_subscribers.TryGetValue(uri, out List<EventHandler<LeagueEvent>> eventHandlers))
                eventHandlers.Add(eventHandler);
            else
                _subscribers.Add(uri, new List<EventHandler<LeagueEvent>> { eventHandler });
        }

        public static void Unsubscribe(string uri)
        {
            if (_subscribers.ContainsKey(uri))
                _subscribers.Remove(uri);
        }

        public static void UnsubscribeAll()
        {
            _subscribers.Clear();
        }
    }
}
