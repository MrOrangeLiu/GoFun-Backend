using DivingApplication.Entities;
using DivingApplication.Repositories.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DivingApplication.Controllers
{
    [Authorize(Policy = "VerifiedUsers")]
    public class ChatHub : Hub
    {
        private readonly IUserRepository _userRepository;

        public ChatHub(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public void Send(string name, string message)
        {
            Clients.All.SendAsync("OnMessage", new object[] { name, message });
        }

        public void SendMessageTest(Message message)
        {
            Clients.All.SendAsync("OnMessageObject", new object[] { message });
        }


        //[Authorize(Policy = "VerifiedUsers")]
        public string MethodOneSimpleParameterSimpleReturnValue(Message message)
        {

            Console.WriteLine($"'MethodOneSimpleParameterSimpleReturnValue' invoked. Parameter value: '{message.Content}");
            return message.Content;
        }
    }
}
