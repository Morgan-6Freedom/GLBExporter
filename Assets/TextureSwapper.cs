using GLTFast;
using GLTFast.Export;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureSwapper : MonoBehaviour
{
    [Tooltip("Set the name of the folder where is your original GLB. The folder should be in Assets/")]
    public string originalGLBFolderName = "GLBOriginal";

    [Tooltip("Set the name of the folder where will be your GLB variants. The folder should be in Assets/")]
    public string variantGLBFolderName = "GLBVariants";

    [Tooltip("Set the name of the GLB file. Do not include the extension")]
    public string originalGLBFileName = "RoueTest";

    public List<Texture2D> colorMaps = new List<Texture2D>();
    public List<Texture2D> normalMaps = new List<Texture2D>();
    public List<Texture2D> ormMaps = new List<Texture2D>();

    List<GameObject> variants = new List<GameObject>();

    [ContextMenu("Create Variants")]
    public async void Import()
    {
        //create the importer
        var importer = new GltfImport();
        //import from Assets/originalGLBFolderName/originalGLBFileName.glb
        string path = Path.Combine(Application.dataPath, originalGLBFolderName, originalGLBFileName + ".glb");
        Debug.Log($"importing {path}");
        var success = await importer.Load(path);

        //if it works
        if (success)
        {

            for (int i = 0; i < colorMaps.Count; i++)
            {
                //instantiate the glb as a child of root
                importer.InstantiateMainScene(transform);
                variants.Add(transform.GetChild(i).gameObject);
                transform.GetChild(i).gameObject.transform.position += Vector3.forward * i;
            }
        }
        else
        {
            Debug.LogError("Loading glTF failed!");
        }
    }

    [ContextMenu("Texture Variants")]
    public void CreateVariants()
    {
        //then swap textures
        for (int i = 0; i < variants.Count; i++)
        {
            SwapTextures(variants[i], colorMaps[i], normalMaps[i], ormMaps[i]);
        }

    }

    [ContextMenu("Exports Variants")]
    public void Export()
    {
        //then swap textures
        for (int i = 0; i < variants.Count; i++)
        {
            SimpleExport(i);
        }
    }

    public void SwapTextures(GameObject _object, Texture2D _colorMap, Texture2D _normalMap, Texture2D _ormMap)
    {
        Material mat = _object.GetComponentInChildren<MeshRenderer>().material;
        mat.SetTexture("_MainTex", _colorMap);
        mat.SetTexture("_BumpMap", _normalMap);
        mat.SetTexture("_MetallicGlossMap", _ormMap);
        mat.SetTexture("_OcclusionMap", _ormMap);
    }

    async void SimpleExport(int _nb)
    {
        var exportSettings = new ExportSettings
        {
            format = GltfFormat.Binary,
            fileConflictResolution = FileConflictResolution.Overwrite
        };

        // GameObjectExport lets you create glTFs from GameObject hierarchies
        var export = new GameObjectExport(exportSettings);

        // Add a scene
        export.AddScene(new List<GameObject>() { variants[_nb] }.ToArray());

        // Async glTF export
        bool success = await export.SaveToFileAndDispose(Path.Combine(Application.dataPath, variantGLBFolderName, originalGLBFileName + _nb + ".glb"));

        if (!success)
        {
            Debug.LogError("Something went wrong exporting a glTF");
        }
    }
}
