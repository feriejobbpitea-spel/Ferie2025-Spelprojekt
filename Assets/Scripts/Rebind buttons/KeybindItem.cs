using UnityEngine;

public static class InputSettings
{
    public static KeyCode JumpKey => GetKey("JumpKey", KeyCode.Space);
    public static KeyCode SprintKey => GetKey("SprintKey", KeyCode.LeftShift);
    public static KeyCode TimeSlowKey => GetKey("TimeSlowKey", KeyCode.T);

    public static void SetKey(string keyName, KeyCode newKey)
    {
        PlayerPrefs.SetString(keyName, newKey.ToString());
        PlayerPrefs.Save();
    }

    public static KeyCode GetKey(string keyName, KeyCode defaultKey)
    {
        string saved = PlayerPrefs.GetString(keyName, defaultKey.ToString());
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), saved);
    }
}
