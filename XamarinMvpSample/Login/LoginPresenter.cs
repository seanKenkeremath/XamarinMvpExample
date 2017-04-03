using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace XamarinMvpSample
{
	public class LoginPresenter : BasePresenter<ILoginView>
	{
		private ILoginInteractor _interactor;

		private string _username;
		private string _password;
		private bool _pendingLoginRequest;

		public LoginPresenter(ILoginView view, ILoginInteractor interactor) : base(view)
		{
			_interactor = interactor;
		}

		public override void Destroy()
		{
			_interactor.Destroy();
		}

		public override Task Init()
		{

			View?.OnLoginButtonEnabled(true);
			return Task.FromResult(0);
		}

		private bool HasValidInput()
		{
			return !string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password);
		}

		public void UpdateUsername(string username)
		{
			_username = username;
			ValidateInput();
		}

		public void UpdatePassword(string password)
		{
			_password = password;
			ValidateInput();
		}

		private void ValidateInput()
		{
			View?.OnLoginButtonEnabled(HasValidInput());
		}

		public async Task Login()
		{
			if (!HasValidInput())
			{
				return;
			}

			View?.OnShouldClearError();
			_pendingLoginRequest = true;
			View?.OnWaiting();
			Result<bool> result = await _interactor.Login(_username, _password);
			View?.OnStopWaiting();
			_pendingLoginRequest = false;

			if (result.Error == Error.None)
			{
				View?.OnGoToNextScreen();
			}
			else if (result.Error == Error.InvalidCredentials)
			{
				View?.OnInvalidCredentials(Strings.InvalidCredentialsError);
			}
		}

		public override async Task RestoreState(IList<string> savedState)
		{
			_username = savedState[0];
			_password = savedState[1];
			_pendingLoginRequest = JsonConvert.DeserializeObject<bool>(savedState[2]);
			if (_pendingLoginRequest)
			{
				await Login();
			}
		}

		public override IList<string> SaveState()
		{
			List<string> state = new List<string>();
			state.Add(_username);
			state.Add(_password);
			state.Add(JsonConvert.SerializeObject(_pendingLoginRequest));
			return state;
		}
	}
}
