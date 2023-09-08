using UnityEngine;
using Steamworks.Data;
using UnityEngine.AI;

public static class Extensions
{
    public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            Mathf.Round(vector3.z * multiplier) / multiplier);
    }

    public static Texture2D Convert(this Image image)
    {
        // Create a new Texture2D
        var avatar = new Texture2D((int)image.Width, (int)image.Height, TextureFormat.ARGB32, false);

        // Set filter type, or else its really blury
        avatar.filterMode = FilterMode.Trilinear;

        // Flip image
        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                var p = image.GetPixel(x, y);
                avatar.SetPixel(x, (int)image.Height - y, new UnityEngine.Color(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f));
            }
        }
        avatar.Apply();
        return avatar;
    }

    public static float CalculateDistance(this NavMeshPath path)
    {
        float distance = 0f;

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }

        return distance;
    }

    public static T[] FindObjectsByType<T>(this Object o) where T : Object //нельзя делать статические методы для расширения А ОЧЕНЬ ЖАЛЬ
    {
        return Object.FindObjectsByType<T>(FindObjectsSortMode.None);
    }

    public static void ChangeListener(this SteamAudio.SteamAudioManager steamAudioManager, Transform listener)
    {
        steamAudioManager.mListener = listener;
    }
}
