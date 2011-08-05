using System;
using Ninject.Modules;

namespace Mural
{
	public class MuralModule : NinjectModule
	{
		public override void Load()
		{
			Bind<ILineConsumer>().To<RedirectingParser>();
			Bind<TelnetListener>().ToSelf();
		}
	}
}

