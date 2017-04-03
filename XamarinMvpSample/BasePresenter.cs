using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XamarinMvpSample
{
	public abstract class BasePresenter
	{
		public IBaseView View { get; set; }


		public BasePresenter(IBaseView view)
		{
			View = view;
		}

		//Any initial calls to the view or api calls should go here
		//Do not put initialization in the constructor because Android may need to recreate the presenter from a saved state
		public abstract Task Init();

		//Call any destroy methods in interactor(s)
		public abstract void Destroy();

		//These methods are to allow the presenter to be restored properly on Android when the View is killed by the system
		public abstract Task RestoreState(IList<string> savedState);
		public abstract IList<string> SaveState();
	}

	public abstract class BasePresenter<V> : BasePresenter where V : IBaseView
	{

		public BasePresenter(V view) : base(view)
		{
		}

		public new V View
		{
			get
			{
				return (V)base.View;
			}
			set
			{
				base.View = value;
			}
		}
	}
}
