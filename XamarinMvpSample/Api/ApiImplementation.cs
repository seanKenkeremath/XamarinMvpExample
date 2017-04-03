using System;
using System.Threading.Tasks;

namespace XamarinMvpSample
{
	public class ApiImplementation : IApi
	{
		//Mock implementation of an API for demo purposes
		//Could use Refit or something else to implement IApi interface in App

		Task<bool> IApi.Login(string username, string password)
		{
			if (username == "User" && password == "CorrectPassword")
			{
				return Task.FromResult(true);
			}
			return Task.FromResult(false);
		}
	}
}
