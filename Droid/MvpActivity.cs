
using System;
using Android.OS;
using Android.Support.V7.App;
using Ninject;

namespace XamarinMvpSample.Droid
{
	public abstract class MvpActivity<T> : AppCompatActivity, IBaseView where T : BasePresenter
	{

		private readonly string PresenterStateKey = "KEY_PRESENTER_STATE";

		protected T Presenter { get; set; }

		protected abstract IBaseView View { get; }
		protected abstract IKernel Kernel { get; }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		protected void CreatePresenter(Bundle savedInstanceState)
		{

			if (GetRetainedPresenter() != null)
			{
				Presenter = GetRetainedPresenter();
				Presenter.View = View;
			}
			else
			{
				Presenter = Kernel.Get<T>();
				if (savedInstanceState != null)
				{
					Presenter.RestoreState(savedInstanceState.GetStringArrayList(PresenterStateKey));
				}
				else
				{
					Presenter.Init();
				}
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (Presenter == null)
			{
				throw new NotImplementedException();
			}
		}

		public override Java.Lang.Object OnRetainCustomNonConfigurationInstance()
		{
			return new CustomNonConfigurationWrapper<T>(Presenter);
		}

		protected T GetRetainedPresenter()
		{
			if (this.LastCustomNonConfigurationInstance != null)
			{
				CustomNonConfigurationWrapper<T> wrapper = (CustomNonConfigurationWrapper<T>)this.LastCustomNonConfigurationInstance;
				return wrapper.Target;
			}
			return default(T);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			Presenter.View = null;
			if (!IsChangingConfigurations)
			{
				Presenter.Destroy();
			}
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			outState.PutStringArrayList(PresenterStateKey, Presenter.SaveState());
		}

		public abstract void OnNetworkError();

	}

	public class CustomNonConfigurationWrapper<T> : Java.Lang.Object
	{
		public readonly T Target;

		public CustomNonConfigurationWrapper(T target)
		{
			Target = target;
		}
	}
}
