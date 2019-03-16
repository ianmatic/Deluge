using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderManager : MonoBehaviour
{
    public Shader unlitDefault;
    public Shader unlitColorShift;
    public GameObject testObject;

    private const string RED_VEC_ADDRESS = "Vector1_2AE1A5AA";
    private const string GREEN_VEC_ADDRESS = "Vector1_6C03338A";
    private const string BLUE_VEC_ADDRESS = "Vector1_71E5B973";

    void Update()
    {
        // Testing shader swapping/update stuff
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Untint(testObject);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            TintRed(testObject);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            TintGreen(testObject);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            TintBlue(testObject);
        }
    }

    /// <summary>
    /// Removes the tint from a material
    /// </summary>
    /// <param name="target"></param>
    public void Untint(GameObject target)
    {
        target.GetComponent<MeshRenderer>().material.shader = unlitDefault;

        /*
        target.GetComponent<MeshRenderer>().material.SetFloat("Vector1_2AE1A5AA", 1.0f);    // Red Value
        target.GetComponent<MeshRenderer>().material.SetFloat("Vector1_6C03338A", 1.0f);    // Green Value
        target.GetComponent<MeshRenderer>().material.SetFloat("Vector1_71E5B973", 1.0f);    // Blue Value
        */

        /* Might revisit later
        // Get the target's instance ID
        int id = target.GetInstanceID();

        // If there's nothing in the dictionary, it's already the normal texture
        // If there is something w/ that id, reset to that material 
        if (matColorStorage[id] != null)
        {
            target.GetComponent<MeshRenderer>().material.mainTexture = matColorStorage[id];
        }
        */
    }

    /// <summary>
    /// Tints a material red
    /// </summary>
    /// <param name="target"></param>
    public void TintRed(GameObject target)
    {
        target.GetComponent<MeshRenderer>().material.shader = unlitColorShift;
        target.GetComponent<MeshRenderer>().material.SetFloat(RED_VEC_ADDRESS, 1.0f);    // Red Value
        target.GetComponent<MeshRenderer>().material.SetFloat(GREEN_VEC_ADDRESS, 1.4f);    // Green Value
        target.GetComponent<MeshRenderer>().material.SetFloat(BLUE_VEC_ADDRESS, 1.4f);    // Blue Value
    }

    /// <summary>
    /// Tints a material green
    /// </summary>
    /// <param name="target"></param>
    public void TintGreen(GameObject target)
    {
        target.GetComponent<MeshRenderer>().material.shader = unlitColorShift;
        target.GetComponent<MeshRenderer>().material.SetFloat(RED_VEC_ADDRESS, 1.4f);    // Red Value
        target.GetComponent<MeshRenderer>().material.SetFloat(GREEN_VEC_ADDRESS, 1.0f);    // Green Value
        target.GetComponent<MeshRenderer>().material.SetFloat(BLUE_VEC_ADDRESS, 1.4f);    // Blue Value
    }

    /// <summary>
    /// Tints a material blue
    /// </summary>
    /// <param name="target"></param>
    public void TintBlue(GameObject target)
    {
        target.GetComponent<MeshRenderer>().material.shader = unlitColorShift;
        target.GetComponent<MeshRenderer>().material.SetFloat(RED_VEC_ADDRESS, 1.4f);    // Red Value
        target.GetComponent<MeshRenderer>().material.SetFloat(GREEN_VEC_ADDRESS, 1.4f);    // Green Value
        target.GetComponent<MeshRenderer>().material.SetFloat(BLUE_VEC_ADDRESS, 1.0f);    // Blue Value
    }
}
