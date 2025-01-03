using UnityEngine;

public static class SaveHandler
{
    public static void SaveValue<T>(string _Key, T _Value, bool _InstantDiskSave = true)
    {
        switch (_Value)
        {
            case bool boolValue:
                PlayerPrefs.SetInt(_Key, boolValue ? 1 : 0);
                break;

            case int intValue:
                PlayerPrefs.SetInt(_Key, intValue);
                break;

            case string stringValue:
                PlayerPrefs.SetString(_Key, stringValue);
                break;

            case float floatValue:
                PlayerPrefs.SetFloat(_Key, floatValue);
                break;

            default:
                Debug.Log("The value you want to save must be one of the following types : bool, int, float, string.");
                break;
        }

        if (_InstantDiskSave)
            SaveAll();
    }

    public static void SaveAll()
    {
        PlayerPrefs.Save();
    }

    public static T LoadValue<T>(string _Key, T _Default)
    {
        T returnValue = _Default;

        switch (_Default)
        {
            case bool boolDefault:
                returnValue = (T)(object)(PlayerPrefs.GetInt(_Key, boolDefault ? 1 : 0) == 1);
                break;

            case int intDefault:
                returnValue = (T)(object)PlayerPrefs.GetInt(_Key, intDefault);
                break;

            case string stringDefault:
                returnValue = (T)(object)PlayerPrefs.GetString(_Key, stringDefault);
                break;

            case float floatDefault:
                returnValue = (T)(object)PlayerPrefs.GetFloat(_Key, floatDefault);
                break;

            default:
                Debug.Log("The value you want to load must be one of the following types : bool, int, float, string.");
                break;
        }

        return returnValue;
    }
}
