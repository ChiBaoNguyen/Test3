using System.Security.Cryptography;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Root.Models;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Website.ViewModels.ChatMessage;
using Website.ViewModels.Driver;
using Website.ViewModels.User;

namespace Service.Services
{
	public interface IChatMessageService
	{
		List<DriverViewModel> GetDriversForChat();
		List<ChatMessageViewModel> GetChatMessages();
		DriverMessageList GetDriverChatMessages(string driverC);
        DriverMessageList GetDriverChatMessagePage(string driverC, int page); 
		bool SaveDriverMessage(ChatMessagePayload payload);
		List<UserViewModel> SaveObserverMessage(ChatMessagePayload payload);
		void SaveChatMessage();
	}

	public class ChatMessageService: IChatMessageService
	{
		private readonly IChatMessageRepository _chatMessageRepository;
		private readonly IUserRepository _userRepository;
		private readonly IDriverRepository _driverRepository;
		private readonly IUnitOfWork _unitOfWork;

		public ChatMessageService(IChatMessageRepository chatMessageRepository, IUserRepository userRepository, 
									IDriverRepository driverRepository, IUnitOfWork unitOfWork)
		{
			this._chatMessageRepository = chatMessageRepository;
			this._userRepository = userRepository;
			this._driverRepository = driverRepository;
			this._unitOfWork = unitOfWork;
		}

		public void SaveChatMessage()
		{
			_unitOfWork.Commit();
		}

		public List<DriverViewModel> GetDriversForChat()
		{
			var driver = from p in _driverRepository.GetAllQueryable()
				join u in _userRepository.GetAllQueryable() on p.DriverC equals u.DriverC into pu
				from u in pu.DefaultIfEmpty()
				where p.IsActive == "1"
				select new DriverViewModel()
				{
					DriverC = p.DriverC,
					LastN = p.LastN,
					FirstN = p.FirstN,
					IsActive = u.RegistrationId != null || u.RegistrationId != "" ? "1" : "0",
				};
			return driver.Any() ? driver.ToList() : null;
		}

		public List<ChatMessageViewModel> GetChatMessages()
		{
			var nowdt = DateTime.Now;
			var pre24dt = DateTime.Now.AddDays(-1);
			var messages = from p in _chatMessageRepository.GetAllQueryable()
				join u in _userRepository.GetAllQueryable() on p.DriverUserId equals u.Id into pu
				from u in pu.DefaultIfEmpty()
				join d in _driverRepository.GetAllQueryable() on u.DriverC equals d.DriverC into pdu
				from d in pdu.DefaultIfEmpty()
				where p.Create_At > pre24dt && p.Create_At < nowdt
				select new ChatMessageViewModel()
				{
					Id = p.Id,
					UserId = p.UserId,
					DriverUserId = p.DriverUserId,
					DriverN = !p.IsDriver & p.DriverUserId == "ALL" ? "ALL" : (d != null ? d.LastN + " " + d.FirstN : ""),
					Message = p.Message,
					Create_At = p.Create_At,
					IsDriver = p.IsDriver
				};

			// sorting (done with the System.Linq.Dynamic library available on NuGet)
			var messagesOrdered = messages.OrderBy("Create_At descending");

			//var messagesLimited = messagesOrdered.Skip(0).Take(20);

			var result = messagesOrdered.OrderBy("Create_At ascending").ToList();

			return result;
		}
       

		public DriverMessageList GetDriverChatMessages(string driverC)
		{
			var messageList = new DriverMessageList();
			var messages = from p in _chatMessageRepository.GetAllQueryable()
						   join u in _userRepository.GetAllQueryable() on p.DriverUserId equals u.Id into pu
						   from u in pu.DefaultIfEmpty()
						   join d in _driverRepository.GetAllQueryable() on u.DriverC equals d.DriverC into pdu
						   from d in pdu.DefaultIfEmpty()
						   where d.DriverC == driverC || p.DriverUserId == "ALL"
						   select new ChatMessageViewModel()
						   {
							   Id = p.Id,
							   UserId = p.UserId,
							   DriverUserId = p.DriverUserId,
							   DriverN = d != null ? d.LastN + " " + d.FirstN : "",
							   Message = p.Message,
							   Create_At = p.Create_At,
							   IsDriver = p.IsDriver
						   };

			if (messages.Any())
			{
				// sorting (done with the System.Linq.Dynamic library available on NuGet)
				var messagesOrdered = messages.OrderBy("Create_At descending");

				var messagesLimited = messagesOrdered.Skip(0).Take(20);

				var result = messagesLimited.OrderBy("Create_At ascending").ToList();

				messageList.ChatMessages = result;

				return messageList;
			}

			return null;
		}

        public DriverMessageList GetDriverChatMessagePage(string driverC, int page)
        {
            var messageList = new DriverMessageList();
            var messages = from p in _chatMessageRepository.GetAllQueryable()
                           join u in _userRepository.GetAllQueryable() on p.DriverUserId equals u.Id into pu
                           from u in pu.DefaultIfEmpty()
                           join d in _driverRepository.GetAllQueryable() on u.DriverC equals d.DriverC into pdu
                           from d in pdu.DefaultIfEmpty()
                           where d.DriverC == driverC || p.DriverUserId == "ALL"
                           select new ChatMessageViewModel()
                           {
                               Id = p.Id,
                               UserId = p.UserId,
                               DriverUserId = p.DriverUserId,
                               DriverN = d != null ? d.LastN + " " + d.FirstN : "",
                               Message = p.Message,
                               Create_At = p.Create_At,
                               IsDriver = p.IsDriver
                           };

            if (messages.Any())
            {
                // sorting (done with the System.Linq.Dynamic library available on NuGet)
                var messagesOrdered = messages.OrderBy("Create_At descending");

                var messagesLimited = messagesOrdered.Skip(20 * (page-1)).Take(20);

                var result = messagesLimited.OrderBy("Create_At ascending").ToList();

                messageList.ChatMessages = result;

                return messageList;
            }

            return null;
        }

		public bool SaveDriverMessage(ChatMessagePayload payload)
		{
			var user = _userRepository.Query(p => p.DriverC == payload.DriverC).FirstOrDefault();
			if (user != null) {
				var message = new ChatMessage()
				{
					DriverUserId = user.Id,
					Message = payload.Message,
					Create_At = DateTime.Now,
					IsDriver = true
				};

				_chatMessageRepository.Add(message);
				SaveChatMessage();
				return true;
			}

			return false;
		}

		public List<UserViewModel> SaveObserverMessage(ChatMessagePayload payload)
		{
			var users = new List<UserViewModel>();

			var message = new ChatMessage()
			{
				UserId = "Admin",
				Message = payload.Message,
				Create_At = DateTime.Now,
				IsDriver = false
			};
			if (payload.IsToAll)
			{
				message.DriverUserId = "ALL";

				users = (from p in _userRepository.GetAllQueryable()
							select new UserViewModel
									{
										Id = p.Id,
										SmartphonePlatform = p.SmartphonePlatform,
										DeviceId = p.RegistrationId,
                                        IsLoggedIn = p.IsLoggedIn
									}).ToList();
			}
			else {
				//var toUser = _userRepository.Query(p => p.DriverC == payload.DriverC).FirstOrDefault();
				var toUser = (from p in _userRepository.GetAllQueryable()
								  where p.DriverC == payload.DriverC
								  select new UserViewModel
								  {
									  Id = p.Id,
									  SmartphonePlatform = p.SmartphonePlatform,
									  DeviceId = p.RegistrationId,
									  IsLoggedIn = p.IsLoggedIn
								  }).FirstOrDefault();
				if (toUser != null)
				{
					message.DriverUserId = toUser.Id;

					users.Add(toUser);
				}
				else {
					return null;
				}
			}

			_chatMessageRepository.Add(message);
			SaveChatMessage();
			return users;
		}
	}
}
