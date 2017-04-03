using System;
using System.Threading.Tasks;

namespace XamarinMvpSample
{
	public class LoginInteractor : ILoginInteractor
	{
		private IApi _api;
		
		public LoginInteractor(IApi api)
		{
			_api = api;
		}

		public void Destroy()
		{
			//would cancel api here
		}

		public async Task<Result<bool>> Login(string username, string password)
		{
			bool success = await _api.Login(username, password);
			if (success)
			{
				return new Result<bool>(true);
			}
			else
			{
				//Incorrect password
				return new Result<bool>(Error.InvalidCredentials);
			}
		}
	}
}
