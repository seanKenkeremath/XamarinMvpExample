
using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Ninject;

namespace XamarinMvpSample.Droid
{
	[Activity(Label = "LoginActivity", MainLauncher = true, Theme="@style/Theme.AppCompat.Light")]
	public class LoginActivity : MvpActivity<LoginPresenter>, ILoginView
	{

		private EditText _usernameInput;
		private EditText _passwordInput;
		private Button _loginButton;
		private TextView _messageText;

		protected override IBaseView View
		{
			get
			{
				return this;
			}
		}

		protected override IKernel Kernel
		{
			get
			{
				return new StandardKernel(new ApplicationModule(), new LoginModule(this));
			}
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Login);


			_loginButton = FindViewById<Button>(Resource.Id.login_button);
			_usernameInput = FindViewById<EditText>(Resource.Id.username_input);
			_passwordInput = FindViewById<EditText>(Resource.Id.password_input);
			_messageText = FindViewById<TextView>(Resource.Id.login_message);

			_usernameInput.TextChanged += UsernameTextChanged;
			_passwordInput.TextChanged += PasswordTextChanged;
			_loginButton.Click += LoginClicked;

			CreatePresenter(savedInstanceState);
		}

		private void UsernameTextChanged(object sender, Android.Text.TextChangedEventArgs e)
		{
			Presenter.UpdateUsername(e.Text.ToString());
		}

		private void PasswordTextChanged(object sender, Android.Text.TextChangedEventArgs e)
		{
			Presenter.UpdatePassword(e.Text.ToString());
		}

		void LoginClicked(object sender, EventArgs e)
		{
			Presenter.Login().RunConcurrent();
		}


		public void OnLoginButtonEnabled(bool enabled)
		{
			_loginButton.Enabled = enabled;
		}

		public void OnWaiting()
		{
			//TODO: since there's no actual delay in the "API" we don't need a loading state
			//No Op
		}

		public void OnStopWaiting()
		{
			//TODO: see above
			//No Op
		}

		public void OnInvalidCredentials(string message)
		{
			_loginButton.Enabled = false;
			_messageText.Text = message;
		}

		public void OnGoToNextScreen()
		{
			Toast.MakeText(this, "Success! TODO: Go to next screen", ToastLength.Long).Show();
		}

		public override void OnNetworkError()
		{
			//TODO
		}

		public void OnShouldClearError()
		{
			_messageText.Text = null;
		}
	}
}
