using System;
using Ninject.Modules;

namespace Mural
{
	public class MuralModule : NinjectModule
	{
		private string _accountFile;
		
		public MuralModule(string accountFile)
		{
			_accountFile = accountFile;
		}
		
		public override void Load()
		{
			Bind<AccountStore>().ToConstant(new SQLiteAccountStore(_accountFile));
			Bind<ICharacterOwnershipIndex>().To<HardcodedCharacterOwnershipIndex>();
			Bind<WorldList>().To<HardcodedWorldList>();
			Bind<WorldIndex>().ToSelf();                    // Depends on WorldList
			Bind<CharacterFactory>().ToSelf();              // Depends on ICharacterOwnershipIndex
			Bind<CharacterSessionIndex>().ToSelf();         // Depends on WorldIndex, CharacterFactory
			Bind<AccountFactory>().ToSelf();                // Depends on ICharacterOwnershipIndex
			Bind<LoginParser>().ToSelf();                   // Depends on AccountFactory, AccountStore, CharacterSessionIndex
			Bind<EchoParser>().ToSelf();
			Bind<ILineConsumer>().To<RedirectingParser>();  // Depends on LoginParser, EchoParser
		}
	}
}

