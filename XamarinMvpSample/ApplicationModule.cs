using System;
using Ninject.Modules;

namespace XamarinMvpSample
{
	public class ApplicationModule : NinjectModule
	{
		public override void Load()
		{
			Bind<IApi>().ToConstant(new ApiImplementation()).InSingletonScope();
		}
	}
}
