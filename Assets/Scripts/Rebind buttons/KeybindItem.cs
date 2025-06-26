using UnityEngine;

public static class InputSettings
{
    public static KeyCode JumpKey => GetKey("bind_Jump", KeyCode.Space);
    public static KeyCode SprintKey => GetKey("bind_Sprint", KeyCode.LeftShift);
    public static KeyCode TimeSlowKey => GetKey("bind_TimeSlow", KeyCode.T);
    public static KeyCode InteractKey => GetKey("bind_Interact", KeyCode.E);

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
