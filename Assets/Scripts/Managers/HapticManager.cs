using MoreMountains.NiceVibrations;
using UnityEngine;

public class HapticManager : MonoBehaviour
{
    public static int vibration = 1;
    public static void SoftVibrate()
    {
        if (PlayerPrefs.HasKey("Vibration"))
            vibration = PlayerPrefs.GetInt("Vibration");
        else
            PlayerPrefs.SetInt("Vibration", 1);

        if (vibration == 1)
            MMVibrationManager.Haptic(HapticTypes.SoftImpact);
       
    }

    public static void SuccesVibrate()
    {
        if (PlayerPrefs.HasKey("Vibration"))
            vibration = PlayerPrefs.GetInt("Vibration");
        else
            PlayerPrefs.SetInt("Vibration", 1);

        if (vibration == 1)
            MMVibrationManager.Haptic(HapticTypes.Success);
       
    }
    
    public static void HeavyVibrate()
    {
        if (PlayerPrefs.HasKey("Vibration"))
            vibration = PlayerPrefs.GetInt("Vibration");
        else
            PlayerPrefs.SetInt("Vibration", 1);

        if (vibration == 1)
            MMVibrationManager.Haptic(HapticTypes.Failure);
       
    }

}