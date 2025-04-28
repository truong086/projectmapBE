using FirebaseAdmin.Messaging;
using projectmap.Common;
using projectmap.Models;
using projectmap.ViewModel;

namespace projectmap.Service
{
    public class NotifitionAppService : INotifitionAppService
    {
        private readonly DBContext _context;
        public NotifitionAppService(DBContext context)
        {
            _context = context;
        }

        public async Task<PayLoad<string>> notifi(string token)
        {
            try
            {
                var messageSend = new Message
                {
                    Token = token,
                    Notification = new Notification()
                    {
                        Title = "💫🕳💫🕳🕳💯 New Plan Notification",
                        Body = "There is a plan from Admin just created"
                    }
                };
                await FirebaseMessaging.DefaultInstance.SendAsync(messageSend);

                return await Task.FromResult(PayLoad<string>.Successfully(Status.SUCCESS));

            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<string>.CreatedFail(ex.Message));
            }
        }
    }
}
