using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

class EditorSetting : ScriptableObject
{
    public VisualTreeAsset behaviourTreeXml;
    public StyleSheet behaviourTreeStyle;
    public StyleSheet grapthViewBackgroundStyle;
    public VisualTreeAsset nodeXml;
    public TextAsset scriptTemplateActionNode; 
    public TextAsset scriptTemplateCompositeNode; 
    public TextAsset scriptTemplateDecoratorNode;
    public string newNodeBasePath = "Assets/"; 

    static EditorSetting FindSettings()
    {
        var guids = AssetDatabase.FindAssets("t:EditorSetting");
        if ( guids.Length > 1 )
        {
            Debug.LogWarning($"Found multiple settings files, using the first.");
        }

        switch ( guids.Length )
        {
            case 0:
                return null;
            default:
                var path = AssetDatabase.GUIDToAssetPath(guids [0]);
                return AssetDatabase.LoadAssetAtPath<EditorSetting>(path);
        }
    }

    internal static EditorSetting GetOrCreateSettings()
    {
        var settings = FindSettings();
        if ( settings == null )
        {
            settings = ScriptableObject.CreateInstance<EditorSetting>();
            AssetDatabase.CreateAsset(settings, "Assets/Settings/Scriptable/EditorSetting.asset");    
            AssetDatabase.SaveAssets();
        }
        return settings;
    }

    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }
}

static class MyCustomSettingsUIElementsRegister
{
    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider()
    {
        var provider = new SettingsProvider("Project/CustomEditorSetting", SettingsScope.Project)
        {
            label = "에디터 설정값 세팅",

            activateHandler = ( searchContext, rootElement ) => {
                var settings = EditorSetting.GetSerializedSettings();

                var title = new Label()
                {
                    text = "에디터 설정값 세팅"
                };
                title.AddToClassList("title");
                rootElement.Add(title);

                var properties = new VisualElement()
                {
                    style =
                    {
                        flexDirection = FlexDirection.Column
                    }
                };
                properties.AddToClassList("setting-list");
                rootElement.Add(properties);

                properties.Add(new InspectorElement(settings));

                rootElement.Bind(settings);
            },
        };

        return provider;
    }
}