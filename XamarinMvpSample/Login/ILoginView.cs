using System;
namespace XamarinMvpSample
{
	public interface ILoginView : IBaseView
	{
		void OnLoginButtonEnabled(bool enabled);
		void OnWaiting();
		void OnStopWaiting();
		void OnInvalidCredentials(string message);
		void OnGoToNextScreen();
		void OnShouldClearError();
	}
}
