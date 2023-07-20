using GenifyStudio.Scripts.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefsManager
{
    private static string KEY_SETTING = "SETTING_KEY";
    public static Setting GetCurrentSetting()
    {
        string data = PlayerPrefs.GetString(KEY_SETTING);
        if (string.IsNullOrEmpty(data))
        {
            return new Setting();
        }
        return JsonConvert.DeserializeObject<Setting>(data);
    }

    public static void UpdateSetting(Setting setting)
    {
        PlayerPrefs.SetString(KEY_SETTING, JsonConvert.SerializeObject(setting ?? new Setting()));
    }
}
