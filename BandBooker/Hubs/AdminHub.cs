using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BandBooker.Hubs
{
    public class AdminHub : Hub
    {
        public async Task SyncMusicianBio(int musicianId, string bio)
        {
            await Clients.Others.SendAsync("ReceiveSyncMusicianBio", musicianId, bio);
        }
    }
}
