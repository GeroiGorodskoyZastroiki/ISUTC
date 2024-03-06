using Sirenix.OdinInspector;
using UnityEngine;

public class MaterialDowngrader : MonoBehaviour
{
    [Button("Downgrade Materials")]
    private void Downgrade()
    {
        //Material[] materials = Resources.LoadAll("", typeof(Material)) as Material[];
        //foreach (Material mat in materials)
        //    if (mat.shader == Shader.Find("Hidden/InternalErrorShader"))
        //    {
        //        Debug.Log(mat.name);
        //        mat.shader = Shader.Find("Standard");
        //    }
        //Debug.Log("This materials were downgraded!");

        //string[] subfolders = System.IO.Directory.GetDirectories("Resources");

        //foreach (string subfolder in subfolders)
        //{
        //    LoadMaterialsRecursively(subfolder);
        //}

        //string[] files = System.IO.Directory.GetFiles(folderPath, "*.mat");
        //foreach (string file in files)
        //{
        //    string materialPath = file.Substring(Application.dataPath.Length - "Assets".Length);
        //    Material[] loadedMaterials = Resources.LoadAll<Material>(materialPath);

        //    foreach (Material material in loadedMaterials)
        //    {
        //        // Делаем что-то с каждым загруженным материалом, например, применяем его к объекту
        //        // Например:
        //        // gameObject.GetComponent<Renderer>().material = material;

        //        Debug.Log("Загружен материал: " + material.name);
        //    }
        //}
    }
}
