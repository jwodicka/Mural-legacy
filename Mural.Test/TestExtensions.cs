using System;
using Moq;

namespace Mural.Test
{
	public static class TestExtensions
	{
		public static Mock<IResponseConsumer> WithMockSource(this ILineConsumer session)
		{
			Mock<IResponseConsumer> source = new Mock<IResponseConsumer>();
			session.AddSource(source.Object);
			return source;
		}
		
		public static Mock<ILineConsumer> WithMockSink(this IResponseConsumer session)
		{
			Mock<ILineConsumer> sink = new Mock<ILineConsumer>();
			session.RaiseUserEvent += sink.Object.HandleUserEvent;
			sink.Object.RaiseResponseEvent += session.HandleResponseEvent;
			return sink;
		}
	}
}

