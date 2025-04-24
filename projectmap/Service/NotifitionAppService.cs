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

        public async Task<PayLoad<string>> notifi()
        {
            try
            {
                var token = "d08hM0xEQ8ejXOmT-Q7eqi:APA91bFu6IeTZBvQ3mVdh-mZDxYnK-uMa2rUuHdeM65v4tzs4Y5aBfUSCaQE5wb8WZPEegOQvlEzq2ITpN1UQkmoLSu85khDOluloN93NVi1IVpRybEO5V8";
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
