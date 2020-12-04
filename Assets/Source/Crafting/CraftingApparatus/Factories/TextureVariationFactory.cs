using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Substance.Game;

public class TextureVariationFactory : MonoBehaviour
{
    private Substance.Game.SubstanceGraph substanceGraph;

    public void GetVariationAndApplyToObject(SubstanceGraph sgo, 
                                             GameObject obj)
    {
        //substanceGraph = sgo.Duplicate();
        substanceGraph = sgo;
        int randomValue = Random.Range(0, 2000000);
        GenerateTextureVariation_Simple(randomValue);
        CopyMaterialToObject(obj);
    }


    private void GenerateTextureVariation_Simple(int randomValue)
    {
        substanceGraph.SetInputInt("$randomSeed", randomValue);
        substanceGraph.QueueForRender();
        substanceGraph.RenderSync();
        Substance.Game.Substance.RenderSubstancesSync();
    }

    private void GenerateTextureVariation_Color(string[] colorFieldNames,
                                                Color[] colors)
    {
        if (colorFieldNames.Length != colors.Length)
            return;
        for (int i = 0; i < colorFieldNames.Length; i++)
        {
            substanceGraph.SetInputColor(colorFieldNames[i], colors[i]);
        }
        substanceGraph.QueueForRender();
        substanceGraph.RenderSync();
        Substance.Game.Substance.RenderSubstancesSync();
    }

    private void CopyMaterialToObject(GameObject obj)
    {
        if (obj == null)
            return;

        Material mat = new Material(substanceGraph.material);
        mat.CopyPropertiesFromMaterial(substanceGraph.material);
        mat.name = substanceGraph.name;                                         // TODO: Improve this later

        List<Texture2D> textures = substanceGraph.GetGeneratedTextures();
        for (int i = 0; i < textures.Count; i++)
        {
            Texture2D copiedTexture = CopyTexture(textures[i]);
            string channel = GetChannelNameFromTextureName(textures[i].name);
            SetMaterialTexture_Helper(mat, copiedTexture, channel);
        }
        obj.GetComponent<Renderer>().material = mat;
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
