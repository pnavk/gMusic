using System;
using System.Threading.Tasks;
using SimpleAuth;

namespace TunezApi
{
	public class TunezApi : AuthenticatedApi
	{
		public TunezApi (string identifier, System.Net.Http.HttpMessageHandler handler)
			:  base (identifier, handler)
		{
			BaseAddress = new Uri ("http://127.0.0.1:51986");
		}

		protected override Task<Account> PerformAuthenticate()
		{
			return Task.FromResult (new Account {
				Identifier = Identifier,
			});
		}

		protected override Task<bool> RefreshAccount(Account account)
		{
			return Task.FromResult (true);
		}
	}
}

