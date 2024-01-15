using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(ArrayLayout))]
public class CustPropertyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.PrefixLabel(position, label);
		Rect newposition = position;
		newposition.y += 144f;
		SerializedProperty data = property.FindPropertyRelative("rows");
        //data.rows[0][]
        if (PotionBoard.Instance != null)
        {
			if (data.arraySize != PotionBoard.Instance.height)
				data.arraySize = PotionBoard.Instance.height;
			for (int j = 0; j < PotionBoard.Instance.height; j++)
			{
				SerializedProperty row = data.GetArrayElementAtIndex(j).FindPropertyRelative("row");
				newposition.height = 18f;
				if (row.arraySize != PotionBoard.Instance.width)
					row.arraySize = PotionBoard.Instance.width;
				newposition.width = position.width / PotionBoard.Instance.width;
				for (int i = 0; i < PotionBoard.Instance.width; i++)
				{
					EditorGUI.PropertyField(newposition, row.GetArrayElementAtIndex(i), GUIContent.none);
					newposition.x += newposition.width;
				}

				newposition.x = position.x;
				newposition.y -= 18f;
			}
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return 18f * 10;
	}
}