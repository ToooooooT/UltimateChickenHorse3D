//---------------------------
//主要功能：提取预制体缩略图
//---------------------------
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class LoadPrefabTexture : MonoBehaviour
{
    private const string FOLDERPATH = "Item";

    string GetTotalPath(string folder)
    {
        // 设置输出路径
        string path;
        path = Application.dataPath + "/../Assets/Resources/" + _prefabPath;
        path += folder;
        path += "/";
        return path;
    }

    string GetAssetPath(string folder)
    {
        // 设置输出路径
        string path;
        path = "Assets/Resources/" + _prefabPath;
        path += folder;
        path += "/";
        return path;
    }

    void Start()
    {
        // 提取缩略图
        // LoadTexture();

        // 清空背景色(等文件生成完)
        // Invoke("ClearBackground", _count * 0.2f);
        ClearBackground();
    }

    GameObject[] LoadAllPrefabsInFolder(string folderPath) {
        List<GameObject> prefabs = new();
        Object[] loadedObjects = Resources.LoadAll(folderPath);

        foreach (Object obj in loadedObjects) {
            if (obj is GameObject) {
                prefabs.Add(obj as GameObject);
            }
        }

        return prefabs.ToArray();
    }

    Texture2D RenderPrefabToTexture(GameObject prefab) {
        GameObject cameraObject = new GameObject("RenderCamera");
        Camera camera = cameraObject.AddComponent<Camera>();

        // Set up a render texture
        RenderTexture renderTexture = new RenderTexture(256, 256, 24);
        camera.targetTexture = renderTexture;

        // Create an instance of the prefab
        GameObject instance = Instantiate(prefab);
        instance.transform.position = new Vector3(0, 0, 0);

        // Render the prefab onto the texture
        camera.Render();

        // Read pixels from the render texture
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // Clean up
        DestroyImmediate(instance);
        DestroyImmediate(cameraObject);
        RenderTexture.active = null;

        return texture;
    }

    // 提取缩略图
    public void LoadTexture() {
        GameObject[] objects = LoadAllPrefabsInFolder(FOLDERPATH);
        for (int i = 0; i < objects.Length; i++) {
            Texture2D tex = AssetPreview.GetAssetPreview(objects[i]);
            // Texture2D tex = RenderPrefabToTexture(objects[i]);
            Debug.Log(objects[i].name);

            if (tex != null) {
                byte[] bytes = tex.EncodeToPNG();
                string totalPath = "Assets/_Assets/Textures/ItemWithBackground/" + objects[i].name + ".png";
                Debug.Log(totalPath);
                File.WriteAllBytes(totalPath, bytes);
                ++_count;
            }
        }
    }

    // 清空背景色
    public void ClearBackground()
    {
        // Assets/_Assets/Textures/ItemWithBackground
        string basepath = "Assets/_Assets/Textures/" + _bgFolder + "/";
        DirectoryInfo directory = new(basepath);
        Debug.Log(directory);
        FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);//查找改路径下的所有文件夹，包含子文件夹

        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].Name.EndsWith(".png")) {
                continue;
            }

            string strBaseName = files[i].Name.Replace(".png", "");
            string totalPath = basepath + strBaseName + ".png";
            Debug.Log(totalPath);
            // if (File.Exists(totalPath))
            // {
            //     TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(GetAssetPath(_bgFolder) + strBaseName + ".png"); // 获取文件
            //     importer.textureCompression = TextureImporterCompression.Uncompressed;
            //     //importer.textureType = TextureImporterType.Sprite; // 修改属性
            //     importer.SaveAndReimport(); // 一定要记得写上这句
            // }

            // m_CurrentTexturePath = GetAssetPath(_bgFolder) + files[i].Name;
            Texture2D texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(totalPath);
            texture2D = DeCompress(texture2D);

            for (int m = 0; m < texture2D.width; m++)
            {
                for (int n = 0; n < texture2D.height; n++)
                {
                    Color color = texture2D.GetPixel(m, n);
                    if (color == _srcColor)
                    {
                        texture2D.SetPixel(m, n, _dstColor);
                    }

                }
            }

            // 设置透明
            if (!texture2D.alphaIsTransparency)
            {
                texture2D.alphaIsTransparency = true;
            }

            //实际应用前面的SetPixel和Setpixels的更改，注意应用的时机，要在处理完一张图片之后再进行应用
            texture2D.Apply();
            byte[] bytes = texture2D.EncodeToPNG();


            using FileStream fileStream = new("Assets/_Assets/Textures/Item/" + strBaseName + ".png", FileMode.Create, FileAccess.Write);
            fileStream.Write(bytes, 0, bytes.Length);
        }
    }

    // 解压缩图片
    public Texture2D DeCompress(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }

    [Header("手动取一下缩略图的背景色")]
    public Color _srcColor = new(0, 217, 255, 0); // 要改的原始颜色

    [Header("改后的背景色（要透明就把alpha调成0）")]
    public Color _dstColor; // 改后的目标颜色

    // prefab预制体必须放在Resources目录下
    [Header("Resources文件夹下的相对路径")]
    public string _prefabPath;

    [Header("带背景的缩略图文件夹")]
    public string _bgFolder = "ItemWithBackground"; // 输出文件夹
    [Header("缩略图文件夹")]
    public string _targetFolder = "Textures"; // 输出文件夹

    private string m_CurrentTexturePath = null;//具体图片
    private int _count = 0;
}