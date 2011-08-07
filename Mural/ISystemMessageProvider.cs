using System;
using System.Collections.Generic;

namespace Mural
{
	/// <summary>
	/// Provides standard system messages (login splashscreen, logout message, etc),
	/// localized by the type of endpoint they are delivered to.
	/// </summary>
	public interface ISystemMessageProvider
	{
		List<string> GetMessage(string messageType, string endpointType);
	}
}

