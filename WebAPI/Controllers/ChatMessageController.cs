using Microsoft.AspNet.SignalR;
using Newtonsoft.Json.Linq;
using PushSharp.Apple;
using PushSharp.Core;
using PushSharp.Google;
using Service.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Website.ViewModels.ChatMessage;
using System.Web.Script.Serialization;
using Website.ViewModels.User;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
	public class ChatMessageController : MobileBaseController
	{
		private string FIREBASE_URL = "https://fcm.googleapis.com/fcm/send";
		//Sender ID
		private string SENDER_ID = System.Configuration.ConfigurationManager.AppSettings["FCM-SENDER-ID"];
		//Server Key
		private string API_KEY = System.Configuration.ConfigurationManager.AppSettings["FCM-SERVER-KEY"];
		//"AIzaSyCFu9fX-p29ZwPji0N37aepz75wNoCPN3M";
		// GET: ChatMessage

		public static string APNSCertificateFolder = "~/Certificates";
		public static string APNSCertificateFile = "BGTDistributionCertificates.p12";

		public IChatMessageService _chatMessagesService;
		public IUserService _userService;
		public IDriverService _driverService;
		public ChatMessageController() { }

		public ChatMessageController(IChatMessageService chatMessagesService, IUserService userService, IDriverService driverService)
		{
			this._chatMessagesService = chatMessagesService;
			this._userService = userService;
			this._driverService = driverService;
		}

		[HttpGet]
		[Route("api/ChatMessage/GetDriversForChat")]
		public HttpResponseMessage GetDriversForChat()
		{
			return base.BuildSuccessResult(HttpStatusCode.OK, _chatMessagesService.GetDriversForChat());
		}

		[HttpGet]
		[Route("api/ChatMessage/GetAllChatMessages")]
		public HttpResponseMessage GetAllChatMessages()
		{
			return base.BuildSuccessResult(HttpStatusCode.OK, _chatMessagesService.GetChatMessages());
		}

		[HttpGet]
		[Route("api/ChatMessage/GetDriverChatMessages")]
		public HttpResponseMessage GetDriverChatMessages(string driverC)
		{
			return base.BuildSuccessResult(HttpStatusCode.OK, _chatMessagesService.GetDriverChatMessages(driverC));
		}

        [HttpGet]
        [Route("api/ChatMessage/GetDriverChatMessagePage")]
        public HttpResponseMessage GetDriverChatMessages(string driverC, int page)
        {
            return base.BuildSuccessResult(HttpStatusCode.OK, _chatMessagesService.GetDriverChatMessagePage(driverC,page));
        }

		[HttpGet]
		[Route("api/ChatMessage/RegisterDeviceRegistrationId")]
		public HttpResponseMessage RegisterDeviceRegistrationId(string driverC, string registrationId, string platform)
		{
			return base.BuildSuccessResult(HttpStatusCode.OK, _userService.RegisterDeviceRegistrationId(driverC, registrationId, platform));
		}

		[HttpPost]
		[Route("api/ChatMessage/DriverSendMessage")]
		public HttpResponseMessage DriverSendMessage(ChatMessagePayload payLoad)
		{
			var status = _chatMessagesService.SaveDriverMessage(payLoad);
			if (status)
			{
				//Will implement SignalR to push message to web application here
				//SignalR to All Web clients

				var driverN = _driverService.GetDriverName(payLoad.DriverC);

				var context = GlobalHost.ConnectionManager.GetHubContext<ChatMessageHub>();
				context.Clients.All.chatMessageHub(driverN, payLoad.Message, DateTime.Now, true);

				var notifyContext = GlobalHost.ConnectionManager.GetHubContext<NotifyMessageHub>();
				notifyContext.Clients.All.notifyMessageHub();
			}

			return base.BuildSuccessResult(HttpStatusCode.OK, status);
		}

		[HttpPost]
		[Route("api/ChatMessage/ObserverSendMessage")]
		public HttpResponseMessage ObserverSendMessage(ChatMessagePayload payLoad)
		{
			var users = _chatMessagesService.SaveObserverMessage(payLoad);

			//---------------------------
			// ANDROID GCM NOTIFICATIONS
			//---------------------------
			var gcmDevices = (from p in users
				where p.SmartphonePlatform == "android"
				select new
				{
					DeviceId = p.DeviceId,
					IsLoggedIn = p.IsLoggedIn
				}).ToList();

			if (gcmDevices.Any())
			{
				//Android_PushNotification(gcmDeviceIds, payLoad.Message);
				foreach (var gcmDevice in gcmDevices)
				{
					if (int.Parse(gcmDevice.IsLoggedIn) == 1)
					{
						PushNotification(gcmDevice.DeviceId, payLoad.Message);
					}
					
				}
			}

			////-------------------------
			//// APPLE iOS NOTIFICATIONS
			////-------------------------
			var apnsDeviceIds = (from p in users
										where p.SmartphonePlatform == "ios"
                                 select new
                                 {
                                     DeviceId = p.DeviceId,
                                     IsLoggedIn = p.IsLoggedIn
                                 }).ToList();
            if (apnsDeviceIds.Any())
            {
                foreach (var apnsDeviceId in apnsDeviceIds)
                {
                    if (int.Parse(apnsDeviceId.IsLoggedIn) == 1)
                    {
                        IOS_PushNotification(apnsDeviceId.DeviceId, payLoad.Message);
                    }

                }
            }
			//SignalR to All Web clients
			var driverN = payLoad.DriverC != "0" ? _driverService.GetDriverName(payLoad.DriverC) : "ALL";

			var context = GlobalHost.ConnectionManager.GetHubContext<ChatMessageHub>();
			context.Clients.All.chatMessageHub(driverN, payLoad.Message, DateTime.Now, false);

			return base.BuildSuccessResult(HttpStatusCode.OK, true);
		}

		public HttpResponseMessage PushNotification(string deviceId, string message)
        {
            try
            {
                WebRequest wRequest;
                wRequest = WebRequest.Create(FIREBASE_URL);
                wRequest.Method = "POST";
                wRequest.ContentType = "application/json;charset=UTF-8";
				wRequest.Headers.Add(string.Format("Authorization: key={0}", API_KEY));
				wRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

				var data = new
				{
					to = deviceId,
					data = new
					{
						BodyMessage = message,
						Create_At = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"),
						Is_Driver = false
					},
					time_to_live = 600
				};

				var serializer = new JavaScriptSerializer();
				var json = serializer.Serialize(data);
				Byte[] byteArray = Encoding.UTF8.GetBytes(json);
				wRequest.ContentLength = byteArray.Length;



				//var bytes = Encoding.UTF8.GetBytes(postData);
				//wRequest.ContentLength = bytes.Length;

				var stream = wRequest.GetRequestStream();
				//stream.Write(bytes, 0, bytes.Length);
				stream.Write(byteArray, 0, byteArray.Length);
				stream.Close();

				var wResponse = wRequest.GetResponse();
				stream = wResponse.GetResponseStream();
				var reader = new StreamReader(stream);
				var response = reader.ReadToEnd();

				var httpResponse = (HttpWebResponse)wResponse;
				var status = httpResponse.StatusCode.ToString();

				reader.Close();
				stream.Close();
				wResponse.Close();
				

                //TODO check status
            }
            catch (WebException ex)
            {
                Console.Write(ex.Message.ToString());
            }
			return base.BuildSuccessResult(HttpStatusCode.OK, true);
        }

		public void Android_PushNotification(List<string> deviceIds, string message)
		{
			//Send message to devices
			// Configuration
			var SENDER_ID = System.Configuration.ConfigurationManager.AppSettings["GCM-SENDER-ID"];
			var AUTH_TOKEN = System.Configuration.ConfigurationManager.AppSettings["AUTH-TOKEN"];
			var config = new GcmConfiguration(SENDER_ID, AUTH_TOKEN, null);
			config.GcmUrl = "https://android.googleapis.com/gcm/send";
			// Create a new broker
			var gcmBroker = new GcmServiceBroker(config);

			// Wire up events
			gcmBroker.OnNotificationFailed += (notification, aggregateEx) =>
			{

				aggregateEx.Handle(ex =>
				{

					// See what kind of exception it was to further diagnose
					if (ex is GcmNotificationException)
					{
						var notificationException = (GcmNotificationException)ex;

						// Deal with the failed notification
						var gcmNotification = notificationException.Notification;
						var description = notificationException.Description;

						//Console.WriteLine($"GCM Notification Failed: ID={gcmNotification.MessageId}, Desc={description}");
					}
					else if (ex is GcmMulticastResultException)
					{
						var multicastException = (GcmMulticastResultException)ex;

						foreach (var succeededNotification in multicastException.Succeeded)
						{
							//Console.WriteLine($"GCM Notification Failed: ID={succeededNotification.MessageId}");
						}

						foreach (var failedKvp in multicastException.Failed)
						{
							var n = failedKvp.Key;
							var e = failedKvp.Value;

							//Console.WriteLine($"GCM Notification Failed: ID={n.MessageId}, Desc={e.Message}");
						}

					}
					else if (ex is DeviceSubscriptionExpiredException)
					{
						var expiredException = (DeviceSubscriptionExpiredException)ex;

						var oldId = expiredException.OldSubscriptionId;
						var newId = expiredException.NewSubscriptionId;

						//Console.WriteLine($"Device RegistrationId Expired: {oldId}");

						if (!string.IsNullOrWhiteSpace(newId))
						{
							// If this value isn't null, our subscription changed and we should update our database
							//Console.WriteLine($"Device RegistrationId Changed To: {newId}");
						}
					}
					else if (ex is RetryAfterException)
					{
						var retryException = (RetryAfterException)ex;
						// If you get rate limited, you should stop sending messages until after the RetryAfterUtc date
						//Console.WriteLine($"GCM Rate Limited, don't send more until after {retryException.RetryAfterUtc}");
					}
					else
					{
						//Console.WriteLine("GCM Notification Failed for some unknown reason");
					}

					// Mark it as handled
					return true;
				});
			};

			gcmBroker.OnNotificationSucceeded += (notification) =>
			{
				//Console.WriteLine("GCM Notification Sent!");
			};

			// Start the broker
			gcmBroker.Start();

			foreach (var regId in deviceIds)
			{
				if (!string.IsNullOrEmpty(regId))
				{
					// Queue a notification to send
					gcmBroker.QueueNotification(new GcmNotification
					{
						RegistrationIds = new List<string> { regId },
						Data = JObject.Parse("{\"Message\" : \"" + message + "\", \"IsDriver\" : \"false\", \"Create_At\": \"" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt") + "\"}"),
						Priority = GcmNotificationPriority.High,
						TimeToLive = 108,
						DelayWhileIdle = true
					});
				}
			}

			// Stop the broker, wait for it to finish   
			// This isn't done after every message, but after you're
			// done with the broker
			gcmBroker.Stop();
		}

        public HttpResponseMessage IOS_PushNotification(string deviceId, string message)
		{
            try
            {
                WebRequest wRequest;
                wRequest = WebRequest.Create(FIREBASE_URL);
                wRequest.Method = "POST";
                wRequest.ContentType = "application/json;charset=UTF-8";
                wRequest.Headers.Add(string.Format("Authorization: key={0}", API_KEY));
                wRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
                var data = new
                {
                    notification = new
                    {
                        title = "Thông báo",
                        sound = "true",
                        body = message,
                        data = new
                        {
                            BodyMessage = message,
                            Create_At = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"),
                            Is_Driver = false
                        },
                        time_to_live = 600
                    },
                    to = deviceId
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                wRequest.ContentLength = byteArray.Length;



                //var bytes = Encoding.UTF8.GetBytes(postData);
                //wRequest.ContentLength = bytes.Length;

                var stream = wRequest.GetRequestStream();
                //stream.Write(bytes, 0, bytes.Length);
                stream.Write(byteArray, 0, byteArray.Length);
                stream.Close();

                var wResponse = wRequest.GetResponse();
                stream = wResponse.GetResponseStream();
                var reader = new StreamReader(stream);
                var response = reader.ReadToEnd();

                var httpResponse = (HttpWebResponse)wResponse;
                var status = httpResponse.StatusCode.ToString();

                reader.Close();
                stream.Close();
                wResponse.Close();


                //TODO check status
            }
            catch (WebException ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return base.BuildSuccessResult(HttpStatusCode.OK, true);
		}
	}
}