using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace XamarinMvpSample.Tests
{
	[TestFixture]
	public class LoginPresenterTests
	{

		private LoginPresenter _presenter;
		private TestView _view;
		private Mock<ILoginInteractor> _mockInteractor;


		[SetUp]
		public void Setup()
		{
			_view = new TestView();
			_mockInteractor = new Mock<ILoginInteractor>();
			_presenter = new LoginPresenter(_view, _mockInteractor.Object);

			//By default, interactor will return success for any credentials
			//Override as necessary
			_mockInteractor.Setup(interactor => interactor.Login(It.IsAny<string>(), It.IsAny<string>()))
						   .Returns(Task.FromResult(new Result<bool>(true)));
		}

		[Test]
		public async Task TestWaitingCallbackWithMoq()
		{
			Assert.False(_view.Waiting);

			Mock<ILoginView> mockView = new Mock<ILoginView>();

			_presenter = new LoginPresenter(mockView.Object, _mockInteractor.Object);

			_presenter.UpdateUsername("User");
			_presenter.UpdatePassword("Pass");

			await _presenter.Login();

			mockView.Verify(view => view.OnWaiting(), Times.Once());
			mockView.Verify(view => view.OnStopWaiting(), Times.Once());
		}

		[Test]
		public async Task TestWaitingCallback()
		{
			//Interactor will return after a delay (our asserts will happen while waiting for the result)
			_mockInteractor.Setup(interactor => interactor.Login(It.IsAny<string>(), It.IsAny<string>()))
						   .Returns(Task.Run(() =>
						   {
							   Task.Delay(100).Wait();
							   return new Result<bool>(true);
						   }));

			Assert.False(_view.Waiting);

			_presenter.UpdateUsername("User");
			_presenter.UpdatePassword("Pass");

			Task task =  _presenter.Login();

			//At this point, view should be waiting
			Assert.True(_view.Waiting);
			Assert.False(_view.NavigatingToNextScreen);

			await task;

			//Now that we've waited for the task to return, the view should no longer be in a waiting state

			Assert.False(_view.Waiting);
			Assert.True(_view.NavigatingToNextScreen);
		}

		[Test]
		public async Task TestLoginCorrectCredentials()
		{
			Assert.False(_view.NavigatingToNextScreen);

			_presenter.UpdateUsername("User");
			_presenter.UpdatePassword("Pass");
			await _presenter.Login();

			Assert.False(_view.InvalidCredentialsError);
			Assert.True(_view.NavigatingToNextScreen);
		}

		[Test]
		public async Task TestLoginIncorrectCredentials()
		{
			//Interactor will return invalid credentials
			_mockInteractor.Setup(interactor => interactor.Login(It.IsAny<string>(), It.IsAny<string>()))
						   .Returns(Task.FromResult(new Result<bool>(Error.InvalidCredentials)));

			Assert.False(_view.NavigatingToNextScreen);

			_presenter.UpdateUsername("User");
			_presenter.UpdatePassword("Pass");
			await _presenter.Login();

			Assert.True(_view.InvalidCredentialsError);
			Assert.False(_view.NavigatingToNextScreen);
		}

		[Test]
		public async Task TestLoginButtonEnabledWhenValidInput()
		{

			//Button should be enabled only when both fields are filled out
			//Even if we call Login(), nothing should happen when fields are invalid

			Assert.IsFalse(_view.LoginButtonEnabled);

			_presenter.UpdateUsername("User");
			await _presenter.Login();

			Assert.IsFalse(_view.LoginButtonEnabled);

			_presenter.UpdatePassword("Pass");

			Assert.True(_view.LoginButtonEnabled);

			_presenter.UpdatePassword("");
			await _presenter.Login();

			Assert.False(_view.LoginButtonEnabled);
			_mockInteractor.Verify(interactor => interactor.Login(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

			_presenter.UpdatePassword("Pass");
			await _presenter.Login();

			//The interactor should have only been called into once -- when we called Login() with both fields filled out
			_mockInteractor.Verify(interactor => interactor.Login(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

		}

		[Test]
		public async Task TestRestorePresenterInputState()
		{
			Assert.False(_view.NavigatingToNextScreen);

			_presenter.UpdateUsername("User");
			_presenter.UpdatePassword("Pass");

			IList<string> state = _presenter.SaveState();

			//Old presenter is destroyed. create new presenter and reattach to view
			_presenter = new LoginPresenter(_view, _mockInteractor.Object);

			await _presenter.Login();

			//Login is not called. We have not restored state so the presenter does not have the input credentials
			_mockInteractor.Verify(interactor => interactor.Login(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
			Assert.False(_view.NavigatingToNextScreen);

			await _presenter.RestoreState(state);
			await _presenter.Login();

			//Now that we have restored state, Login should have been requested successfully
			_mockInteractor.Verify(interactor => interactor.Login(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

			Assert.True(_view.NavigatingToNextScreen);
		}

		/// <summary>
		/// If a request is made, but the presenter is destroyed and recreated before
		/// the request is completed, it should automatically retry that request when state 
		/// is restored.
		/// </summary>
		/// <returns>The restore state during pending request.</returns>
		[Test]
		public async Task TestRestoreStateDuringPendingRequest()
		{
			//Interactor will return after a delay. We will cancel before this request returns
			_mockInteractor.Setup(interactor => interactor.Login(It.IsAny<string>(), It.IsAny<string>()))
						   .Returns(Task.Run(() =>
						   {
							   Task.Delay(100).Wait();
							   return new Result<bool>(true);
						   }));

			_presenter.UpdateUsername("User");
			_presenter.UpdatePassword("Pass");

			Task task = _presenter.Login();

			//null out view reference so that view will not be updated by this presenter
			IList<string> state = _presenter.SaveState();
			_presenter.View = null;
			await task;

			//Old presenter is destroyed. create new presenter and reattach to view
			_presenter = new LoginPresenter(_view, _mockInteractor.Object);

			Assert.False(_view.NavigatingToNextScreen);

			await _presenter.RestoreState(state);
			Assert.True(_view.NavigatingToNextScreen);

			//Login should have been called twice total. Once in the old presenter and once after restoring
			_mockInteractor.Verify(interactor => interactor.Login(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
		}


		private class TestView : ILoginView
		{

			public bool NavigatingToNextScreen;
			public bool InvalidCredentialsError;
			public bool LoginButtonEnabled;
			public bool NetworkError;
			public bool Waiting;

			public void OnGoToNextScreen()
			{
				NavigatingToNextScreen = true;
			}

			public void OnInvalidCredentials(string message)
			{
				InvalidCredentialsError = true;
			}

			public void OnLoginButtonEnabled(bool enabled)
			{
				LoginButtonEnabled = enabled;
			}

			public void OnNetworkError()
			{
				NetworkError = true;
			}

			public void OnStopWaiting()
			{
				Waiting = false;
			}

			public void OnWaiting()
			{
				Waiting = true;
			}

			public void OnShouldClearError()
			{
				NetworkError = false;
				InvalidCredentialsError = false;
			}
		}

	}
}
