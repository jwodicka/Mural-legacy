using System;
using Ninject.Modules;

namespace Mural
{
	public class MuralModule : NinjectModule
	{
		public override void Load()
		{
			string defaultWorldFile = System.IO.Path.Combine("DefaultDB", "world.db");
			Bind<WorldList>().ToConstant(new SQLiteWorldList(defaultWorldFile));
			string defaultAccountFile = System.IO.Path.Combine("DefaultDB", "account.db");
			Bind<IAccountStore>().ToConstant(new SQLiteAccountStore(defaultAccountFile));
			string defaultCharacterFile = System.IO.Path.Combine("DefaultDB", "character.db");
			Bind<ICharacterOwnership>().ToConstant(new SQLiteCharacterOwnership(defaultCharacterFile));
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

