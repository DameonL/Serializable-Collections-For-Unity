using System;

namespace SerializableCollections.GUI
{
	public class UsePropertyDrawer : Attribute
	{
		public Type PropertyDrawerType;

		public UsePropertyDrawer(Type propertyDrawerType)
		{
			PropertyDrawerType = propertyDrawerType;
		}
	}
}
