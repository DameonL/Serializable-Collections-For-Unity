#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace SerializableCollections.GUI
{
	public static class SerializationReflection
	{
		public static void CallPrivateMethod(SerializedProperty property, string methodName)
		{
			MethodInfo method = null;
			BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
			object pathReference = GetPathReference(property);

			Type currentType = pathReference.GetType();
			while (currentType != null)
			{
				method = currentType.GetMethod(methodName, flags);
				if (method != null)
					break;

				currentType = currentType.BaseType;
			}

			method.Invoke(pathReference, null);
		}

		public static object GetPathReference(SerializedProperty property)
		{
			BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
			PathEntry[] path = SplitPath(property.propertyPath);
			object currentObject = property.serializedObject.targetObject;
			FieldInfo currentField = null;
			bool isInArray = false;
			for (var i = 0; i < path.Length; i++)
			{
				if (path[i].FieldName == "Array")
				{
					isInArray = true;
					continue;
				}

				Type objectType = currentObject.GetType();
				PropertyInfo indexerProperty = null;
				while (objectType != null)
				{
					if (path[i].FieldName == "data" && isInArray)
						indexerProperty = objectType.GetProperty("Item");

					currentField = objectType.GetField(path[i].FieldName, flags);
					if (currentField != null || indexerProperty != null)
						break;

					objectType = objectType.BaseType;
				}

				if (currentField == null && indexerProperty == null)
					UnityEngine.Debug.Log(property.propertyPath + " - " + path[i].FieldName);

				if (indexerProperty != null)
					currentObject = indexerProperty.GetValue(currentObject, new object[] { path[i].ArrayIndex });
				else
					currentObject = currentField.GetValue(currentObject);

				isInArray = false;
			}

			return currentObject;
		}

		public static KeyValuePair<object, FieldInfo> GetPathField(SerializedProperty property)
		{
			BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
			PathEntry[] path = SplitPath(property.propertyPath);
			object currentObject = property.serializedObject.targetObject;
			object lastObject = null;
			FieldInfo currentField = null;
			bool isInArray = false;
			for (var i = 0; i < path.Length; i++)
			{
				if (path[i].FieldName == "Array")
				{
					isInArray = true;
					continue;
				}

				if (currentObject == null)
					UnityEngine.Debug.Log(i);
				Type objectType = currentObject.GetType();
				int maxDepth = 10;
				PropertyInfo indexerProperty = null;
				while (objectType != null)
				{
					if (path[i].FieldName == "data" && isInArray)
						indexerProperty = objectType.GetProperty("Item");

					currentField = objectType.GetField(path[i].FieldName, flags);
					if (currentField != null || indexerProperty != null)
						break;

					objectType = objectType.BaseType;
					maxDepth--;
					if (maxDepth < 0)
						break;
				}

				lastObject = currentObject;
				if (indexerProperty != null)
					currentObject = indexerProperty.GetValue(currentObject, new object[] { path[i].ArrayIndex });
				else
					currentObject = currentField.GetValue(currentObject);

				isInArray = false;
			}

			return new KeyValuePair<object, FieldInfo>(lastObject, currentField);
		}

		public static PathEntry[] SplitPath(string propertyPath)
		{
			string[] splitInput = propertyPath.Split('.');
			System.Collections.Generic.List<PathEntry> entries = new System.Collections.Generic.List<PathEntry>();
			for (var inputIndex = 0; inputIndex < splitInput.Length; inputIndex++)
			{
				int index = -1;
				if (splitInput[inputIndex].EndsWith("]"))
				{
					string indexString = "";
					// Starting at index 5, which is the "Data[" part of the string
					for (var charIndex = 5; charIndex < splitInput[inputIndex].Length - 1; charIndex++)
					{
						indexString += splitInput[inputIndex][charIndex];
					}
					int.TryParse(indexString, out index);

					splitInput[inputIndex] = splitInput[inputIndex].Remove(4, splitInput[inputIndex].Length - 4);
				}
				entries.Add(new PathEntry(splitInput[inputIndex], index));
			}
			return entries.ToArray();
		}

		public struct PathEntry
		{
			public string FieldName;
			public int ArrayIndex;

			public PathEntry(string fieldName, int arrayIndex)
			{
				FieldName = fieldName;
				ArrayIndex = arrayIndex;
			}
		}
	}
}
#endif