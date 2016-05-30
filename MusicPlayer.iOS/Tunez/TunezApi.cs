using System;
using System.Threading.Tasks;
using MusicPlayer.iOS;
using SimpleAuth;

namespace TunezApi
{
	public class TunezApi : AuthenticatedApi
	{
		public TunezApi (string identifier, System.Net.Http.HttpMessageHandler handler)
			:  base (identifier, handler)
		{
			
		}

		protected override async Task<Account> PerformAuthenticate ()
		{
			string address;
			try {
				address = await PopupManager.Shared.GetTextInput ("Enter Tunez server address", "http://test.com:51986");
			} catch (OperationCanceledException) {
				return null;
			}

			BaseAddress = new Uri (address);
			CurrentAccount = new Account {
				Identifier = this.Identifier
			};
			return CurrentAccount;
		}

		protected override Task<bool> RefreshAccount (Account account)
		{
			return Task.FromResult (true);
		}
	}
}

