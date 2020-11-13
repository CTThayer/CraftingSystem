using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Substance.Game;

public class VariationGenerator : MonoBehaviour
{
    public Substance.Game.Substance substance;
    public Substance.Game.SubstanceGraph substanceGraph;
    public GameObject[] testObjs;
    public Color[] testColors;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(substance != null);
        if (substanceGraph == null)
        {
            substanceGraph = substance.graphs[0];
            Debug.Assert(substanceGraph != null);
        }

        //substanceGraph.SetInputVector2Int("$outputsize", 10, 10); // 10 x 10 == 1024 x 1024 - because reasons... Substance API..

        Debug.Assert(testObjs != null);
        Debug.Assert(testColors != null);
        Debug.Assert(testObjs.Length == testColors.Length);

        StartCoroutine(GenerateVariations());

    }

    bool isProcessing = false;
    int count = 0;
    private IEnumerator GenerateVariations()
    {
        isProcessing = Substance.Game.Substance.IsProcessing();
        while (!isProcessing && count < testObjs.Length)
        {
            CopyMaterialToObject(count);
            GenerateTextureVariation(count);
            count++;
            yield return null;
        }
        while (isProcessing)
        {
            yield return null;
        }
        yield return 0;
    }

    private void GenerateTextureVariation(int c)
    {
        substanceGraph.SetInputColor("color_1", testColors[c]);
        int c2 = c + 1;
        c2 = c2 < testColors.Length ? c2 : 0;
        substanceGraph.SetInputColor("color_2", testColors[c2]);
        substanceGraph.SetInputFloat("cracks_density", Random.value);

        substanceGraph.QueueForRender();
        substanceGraph.RenderSync();
        Substance.Game.Substance.RenderSubstancesSync();
    }

    private void CopyMaterialToObject(int objIndex)
    {
        Material mat = new Material(substanceGraph.material);
        mat.CopyPropertiesFromMaterial(substanceGraph.material);
        mat.name = "proceduralMaterial" + objIndex;

        List<Texture2D> textures = substanceGraph.GetGeneratedTextures();
        for (int i = 0; i < textures.Count; i++)
        {
            Texture2D copiedTexture = CopyTexture(textures[i]);
            string channel = GetChannelNameFromTextureName(textures[i].name);
            SetMaterialTexture_Helper(mat, copiedTexture, channel);
        }
        testObjs[objIndex].GetComponent<Renderer>().material = mat;
    }


    private Texture2D CopyTexture(Texture2D original)
    {
        Debug.Log("Texture Mipmap Count: " + original.mipmapCount);

        Debug.Log("Original texture width: " + original.width + 
                  " and original texture height: " + original.height);
        
        Texture2D copy = new Texture2D(original.width, original.height);
        Color[] copiedPixels = original.GetPixels();
        copy.SetPixels(copiedPixels);
        copy.Apply();
        Debug.Log("Texture Copy Complete. copy = " + copy);
        return copy;
    }

    private void SetMaterialTexture_Helper(Material m, Texture2D t)             // TODO: Add cases for other channels (i.e. specular)
    {
        string channelName = GetChannelNameFromTextureName(t.name);
        Debug.Log("Channel Name: " + channelName);
        switch (channelName)
        {
            case "ambientOcclusion":
                m.SetTexture("_OcclusionMap", t);
                break;
            case "baseColor":
                m.SetTexture("_MainTex", t);
                break;
            case "height":
                m.SetTexture("_ParallaxMap", t);
                break;
            case "metallic":
                m.SetTexture("_Metallic", t);
                break;
            case "normal":
                m.SetTexture("_BumpMap", t);
                break;
        }
    }

    private void SetMaterialTexture_Helper(Material m, 
                                           Texture2D t, 
                                           string channelName)
    {
        switch (channelName)                                                    // TODO: Add cases for other channels (i.e. specular)
        {
            case "ambientOcclusion":
                m.SetTexture("_OcclusionMap", t);
                break;
            case "baseColor":
                m.SetTexture("_MainTex", t);
                break;
            case "height":
                m.SetTexture("_ParallaxMap", t);
                break;
            case "metallic":
                m.SetTexture("_Metallic", t);
                break;
            case "normal":
                m.SetTexture("_BumpMap", t);
                break;
        }
    }

    private string GetChannelNameFromTextureName(string textureName)
    {
        if (textureName != null && textureName.Length > 0)
        {
            char[] name = textureName.ToCharArray();
            Stack<char> resultChars = new Stack<char>();
            int index = textureName.Length - 1;
            char currentChar = name[index];
            while (currentChar != ' ')
            {
                resultChars.Push(currentChar);
                index--;
                currentChar = textureName[index];
            }
            char[] chars = new char[resultChars.Count];
            int i = 0;
            while (resultChars.Count > 0)
            {
                chars[i] = resultChars.Pop();
                i++;
            }
            string s = new string(chars);
            return s;
        }
        else return "";
    }


    private void SaveTextureToPNG(Texture2D texture, string fullPath)           // TODO: Test this function!
    {
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);
    }

    private void SaveTextureToPNG(Texture2D texture)                            // TODO: Test this function!
    {
        byte[] bytes = texture.EncodeToPNG();
        string fullPath = Application.persistentDataPath + "\\RuntimeContent\\Textures";
        if (!System.IO.Directory.Exists(fullPath))
        {
            System.IO.Directory.CreateDirectory(fullPath);
        }
        System.IO.File.WriteAllBytes(fullPath, bytes);
    }
}
