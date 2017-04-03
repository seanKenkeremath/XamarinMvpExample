using System;
using Ninject.Modules;

namespace XamarinMvpSample
{
	public class LoginModule : NinjectModule
	{
		private ILoginView _view;

		public LoginModule(ILoginView view)
		{
			_view = view;
		}

		public override void Load()
		{
			Bind<ILoginView>().ToConstant(_view);
			Bind<ILoginInteractor>().To<LoginInteractor>();
			//NOTE: we do not have to explicilty bind LoginPresenter 
			//since we satisfy all of its constructor's dependencies
		}
	}
}
