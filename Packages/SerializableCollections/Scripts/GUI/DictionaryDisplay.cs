#if UNITY_EDITOR
using System;
using UnityEditor;
#endif
using UnityEngine;

namespace SerializableCollections.GUI
{
	public class DictionaryDisplay : CollectionDisplay
	{
#if UNITY_EDITOR
		private GUIStyle headerStyle = new GUIStyle(EditorStyles.miniBoldLabel);

		public DictionaryDisplay() : base()
		{
			headerStyle.alignment = TextAnchor.MiddleCenter;
			headerStyle.fontSize = 12;
		}

		protected override float GetElementHeight(int index, SerializedProperty property, SerializedProperty element)
		{
			if (property == null || property.serializedObject.targetObject == null)
				return 0;

			SerializedProperty keys = property.FindPropertyRelative("keys");
			SerializedProperty values = property.FindPropertyRelative("values");
			var key = keys?.GetArrayElementAtIndex(index);
			var value = values?.GetArrayElementAtIndex(index);
			float keyHeight = (key != null) ? EditorGUI.GetPropertyHeight(key) : 0;
			float valueHeight = (value != null) ? EditorGUI.GetPropertyHeight(value) : 0;
			keyHeight += 5;
			valueHeight += 5;
			float pairHeight = (keyHeight > valueHeight) ? keyHeight : valueHeight;
			return pairHeight;
		}

		protected override Rect DrawCollection(Rect position, SerializedProperty property, SerializedProperty collection, GUIContent label)
		{
			ReorderableCollection.ListStates[GetFullPath(collection)].List.DrawCustomAdd = true;
			return base.DrawCollection(position, property, collection, label);
		}

		protected override float DrawTitle(Rect position, SerializedProperty property, SerializedProperty collection)
		{
			float startY = position.y;
			position.y -= 2;

			Rect keyLabelPosition = position;
			keyLabelPosition.width *= .5f;
			keyLabelPosition.xMin += 10;
			keyLabelPosition.xMax -= 10;

			Rect valueLabelPosition = position;
			valueLabelPosition.width *= .5f;
			valueLabelPosition.x += valueLabelPosition.width;
			valueLabelPosition.width -= 11;

			var fieldValue = SerializationReflection.GetPathField(collection);
			Type[] generics = fieldValue.Value.DeclaringType.GenericTypeArguments;
			string keyType = ObjectNames.NicifyVariableName(generics[0].Name);
			string valueType = ObjectNames.NicifyVariableName(generics[1].Name);

			GUIContent keyLabel = new GUIContent("Key (" + keyType + ")");
			GUIContent valueLabel = new GUIContent("Value (" + valueType + ")");
			keyLabelPosition.height = headerStyle.CalcHeight(keyLabel, position.width) + 5;
			valueLabelPosition.height = keyLabelPosition.height;
			EditorGUI.LabelField(keyLabelPosition, keyLabel, headerStyle);
			EditorGUI.LabelField(valueLabelPosition, valueLabel, headerStyle);
			position.y += valueLabelPosition.height;
			return position.y - startY;
		}

		protected override void DrawElement(Rect position, int index, SerializedProperty property, SerializedProperty collection)
		{
			SerializedProperty keys = property.FindPropertyRelative("keys");
			SerializedProperty values = property.FindPropertyRelative("values");
			SerializedProperty key = keys.GetArrayElementAtIndex(index);
			SerializedProperty value = values.GetArrayElementAtIndex(index);

			position.xMin += 10f;

			Rect keyPosition = position;
			keyPosition.xMax -= position.width * .5f;
			keyPosition.xMax -= 10;
			keyPosition.height = EditorGUI.GetPropertyHeight(key) + 2;

			Rect valuePosition = position;
			valuePosition.xMin += position.width * .5f;
			valuePosition.xMin += 10;
			valuePosition.height = EditorGUI.GetPropertyHeight(value) + 2;

			float labelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = keyPosition.width * .4f;

			UnityEngine.GUI.SetNextControlName(GetFullPath(keys) + "/" + index);
			float textFieldHeight = EditorStyles.textField.fixedHeight;
			EditorStyles.textField.fixedHeight = keyPosition.height;
			if (key.hasChildren && (key.propertyType != SerializedPropertyType.ObjectReference) && (key.propertyType != SerializedPropertyType.String))
				EditorGUI.PropertyField(keyPosition, key, new GUIContent("Key"), true);
			else
				EditorGUI.PropertyField(keyPosition, key, GUIContent.none, true);

			EditorStyles.textField.fixedHeight = valuePosition.height;
			UnityEngine.GUI.SetNextControlName(GetFullPath(values) + "/" + index);
			EditorGUIUtility.labelWidth = valuePosition.width * .4f;
			if (value.hasChildren && (value.propertyType != SerializedPropertyType.ObjectReference) && (value.propertyType != SerializedPropertyType.String))
				EditorGUI.PropertyField(valuePosition, value, new GUIContent("Value"), true);
			else
				EditorGUI.PropertyField(valuePosition, value, GUIContent.none, true);

			EditorStyles.textField.fixedHeight = textFieldHeight;
			EditorGUIUtility.labelWidth = labelWidth;
		}

		protected override float DrawAddElement(Rect position, SerializedProperty property, SerializedProperty collection)
		{
			var tempKey = property.FindPropertyRelative("key");
			var tempValue = property.FindPropertyRelative("value");
			float labelWidth = EditorGUIUtility.labelWidth;
			Rect startPostion = position;

			Rect keyPosition = position;
			keyPosition.xMin += 15;
			keyPosition.xMax -= position.width * .5f + 10;
			keyPosition.height = EditorGUI.GetPropertyHeight(tempKey);

			float fieldHeight = EditorStyles.textField.fixedHeight;
			EditorStyles.textField.fixedHeight = keyPosition.height + 2;
			UnityEngine.GUI.SetNextControlName("New Key Add");
			EditorGUIUtility.labelWidth = keyPosition.width * .4f;
			if (tempKey.hasChildren && (tempKey.propertyType != SerializedPropertyType.ObjectReference) && (tempKey.propertyType != SerializedPropertyType.String))
				EditorGUI.PropertyField(keyPosition, tempKey, new GUIContent(tempKey.displayName), true);
			else
				EditorGUI.PropertyField(keyPosition, tempKey, GUIContent.none);

			Rect valuePosition = position;
			valuePosition.xMin += position.width * .5f + 10;
			valuePosition.xMax -= 15;
			valuePosition.height = EditorGUI.GetPropertyHeight(tempValue);

			EditorStyles.textField.fixedHeight = valuePosition.height + 2;
			EditorGUIUtility.labelWidth = valuePosition.width * .4f;
			if (tempValue.hasChildren && (tempValue.propertyType != SerializedPropertyType.ObjectReference) && (tempValue.propertyType != SerializedPropertyType.String))
				EditorGUI.PropertyField(valuePosition, tempValue, new GUIContent(tempValue.displayName), true);
			else
				EditorGUI.PropertyField(valuePosition, tempValue, GUIContent.none);


			EditorStyles.textField.fixedHeight = fieldHeight;

			EditorGUIUtility.labelWidth = labelWidth;

			float propertyHeight = (keyPosition.height > valuePosition.height) ? keyPosition.height : valuePosition.height;
			position.y += propertyHeight + 5;
			return position.y - startPostion.y;
		}

		protected override void OnReorder(SerializedProperty property, SerializedProperty collection, int oldIndex, int newIndex)
		{
			SerializedProperty keys = property.FindPropertyRelative("keys");
			keys.MoveArrayElement(oldIndex, newIndex);
			base.OnReorder(property, collection, oldIndex, newIndex);
		}
#endif
	}
}