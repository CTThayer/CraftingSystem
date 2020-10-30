using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetSubstanceOutputNames : MonoBehaviour
{
    public Substance.Game.Substance substance;
    public Substance.Game.SubstanceGraph substanceGraph;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(substance != null);
        if (substanceGraph != null)
        {
            substanceGraph = substance.graphs[0];
            Debug.Assert(substanceGraph != null);
        }

        List<Texture2D> textures = substanceGraph.GetGeneratedTextures();
        //string[] channels = new string[textures.Count];
        for (int i = 0; i < textures.Count; i++)
        {
            //channels[i] = textures[i].name;
            Debug.Log(textures[i].name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
