using System;

namespace SerializableCollections.GUI
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class DisplayType : Attribute
	{
		public Type Type;

		public DisplayType(Type displayClass)
		{
			Type = displayClass;
		}
	}
}
