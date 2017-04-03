using System;
using System.Threading.Tasks;

namespace XamarinMvpSample
{
	public interface IApi
	{
		//Returns true if successful, false if not
		Task<bool> Login(string username, string password);
	}
}
