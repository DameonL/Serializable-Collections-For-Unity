using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SerializableCollections.GUI
{
	public class CollectionDrawer
		#if UNITY_EDITOR
		: PropertyDrawer
		#endif
	{
#if UNITY_EDITOR
		private static System.Collections.Generic.Dictionary<Type, CollectionDisplay> DisplayHandlers = new System.Collections.Generic.Dictionary<Type, CollectionDisplay>();

		public CollectionDrawer()
		{
			Type currentType = GetType();
			while (currentType != null)
			{
				foreach (var attribute in GetType().GetCustomAttributes())
				{
					if (attribute is DisplayType)
					{
						DisplayType cast = attribute as DisplayType;
						CollectionDisplay handler = (CollectionDisplay)Activator.CreateInstance(cast.Type);
						if (!DisplayHandlers.ContainsKey(GetType()))
							DisplayHandlers.Add(GetType(), handler);

						break;
					}
				}

				currentType = currentType.BaseType;
			}
		}

		public static void RegisterForDrawer(Type drawerType, Type drawerFor)
		{
			Type attributeUtility = Type.GetType("UnityEditor.ScriptAttributeUtility, UnityEditor");
			Type drawerKeySetType = attributeUtility.GetNestedType("DrawerKeySet", BindingFlags.NonPublic);
			FieldInfo drawerKeySetDrawerType = drawerKeySetType.GetField("drawer");
			FieldInfo drawerKeySetDrawerTarget = drawerKeySetType.GetField("type");
			FieldInfo drawerTypeMapField = attributeUtility.GetField("s_DrawerTypeForType", BindingFlags.Static | BindingFlags.NonPublic);
			object drawerTypeMap = drawerTypeMapField.GetValue(null);
			if (drawerTypeMap == null)
			{
				MethodInfo buildMethod = attributeUtility.GetMethod("BuildDrawerTypeForTypeDictionary", BindingFlags.Static | BindingFlags.NonPublic);
				try
				{
					buildMethod.Invoke(null, null);
				}
				catch (TargetInvocationException)
				{
					// For some reason, this method generates a null argument down the line from a LINQ call. Doesn't seem to affect
					// functionality, so we're ignoring the exception.
				}
				drawerTypeMap = drawerTypeMapField.GetValue(attributeUtility);
			}
			MethodInfo drawerTypeMapAdd = drawerTypeMap.GetType().GetMethod("Add", new Type[] { typeof(Type), drawerKeySetType });
			MethodInfo drawerTypeMapContainsKey = drawerTypeMap.GetType().GetMethod("ContainsKey", new Type[] { typeof(Type) });
			bool containsKey = (bool)drawerTypeMapContainsKey.Invoke(drawerTypeMap, new object[] { drawerFor });
			if (!containsKey)
			{
				object drawerKeySet = Activator.CreateInstance(drawerKeySetType);
				drawerKeySetDrawerType.SetValue(drawerKeySet, drawerType);
				drawerKeySetDrawerTarget.SetValue(drawerKeySet, drawerFor);
				drawerTypeMapAdd.Invoke(drawerTypeMap, new object[] { drawerFor, drawerKeySet });
			}
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property != null && DisplayHandlers.ContainsKey(GetType()))
				DisplayHandlers[GetType()].Render(position, property, label);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (DisplayHandlers.ContainsKey(GetType()))
				return DisplayHandlers[GetType()].GetPropertyHeight(property, label);

			return 0;
		}
#endif
	}
}