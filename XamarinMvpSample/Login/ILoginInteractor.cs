using System;
using System.Threading.Tasks;

namespace XamarinMvpSample
{
	public interface ILoginInteractor : IBaseInteractor
	{
		Task<Result<bool>> Login(string username, string password);
	}
}
