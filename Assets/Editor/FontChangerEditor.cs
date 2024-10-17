using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FontChangerEditor : EditorWindow
{
    public Font newFont;

    // 유니티 에디터 상단 메뉴에 'Tools' 항목을 추가하는 부분
    [MenuItem("Tools/Font Changer")]
    public static void ShowWindow()
    {
        GetWindow<FontChangerEditor>("Font Changer");
    }

    void OnGUI()
    {
        GUILayout.Label("Change all Text Fonts", EditorStyles.boldLabel);
        newFont = (Font)EditorGUILayout.ObjectField("New Font", newFont, typeof(Font), false);

        if (GUILayout.Button("Change Fonts"))
        {
            ChangeAllFonts();
        }
    }

    // 모든 Text 컴포넌트의 폰트를 변경하는 메서드
    void ChangeAllFonts()
    {
        Text[] textComponents = FindObjectsOfType<Text>();

        foreach (Text text in textComponents)
        {
            Undo.RecordObject(text, "Change Font");
            text.font = newFont;
            EditorUtility.SetDirty(text);
        }

        Debug.Log("All fonts have been changed.");
    }
}