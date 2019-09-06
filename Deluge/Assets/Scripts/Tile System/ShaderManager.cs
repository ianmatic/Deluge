using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderManager : MonoBehaviour
{
    public Shader litColorShift;
    public Shader litColorPulse;
    public GameObject testObject;

    private Vector3 tintVec;

    private const string FLAT_VEC3_ADDRESS = "_rbg_input_values";

    private const string FLAT_RED_ADDRESS = "_flat_red_input";
    private const string FLAT_GREEN_ADDRESS = "_flat_green_input";
    private const string FLAT_BLUE_ADDRESS = "_flat_blue_input";

    private const string PULSE_RED_VEC_ADDRESS = "Vector1_2AE1A5AA";
    private const string PULSE_GREEN_VEC_ADDRESS = "Vector1_6C03338A";
    private const string PULSE_BLUE_VEC_ADDRESS = "Vector1_71E5B973";

    /// <summary>
    /// Removes the tint from a material, set different shader based on if mobile or not (for snow)
    /// </summary>
    /// <param name="target"></param>
    public void Untint(GameObject target, bool isStatic)
    {
        if (isStatic)
        {
            target.GetComponent<MeshRenderer>().material.shader = Shader.Find("Shader Graphs/SnowStatic");
        }
        else
        {
            target.GetComponent<MeshRenderer>().material.shader = Shader.Find("Shader Graphs/SnowMobile");
        }

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            TintRedPulse(testObject);
        }
    }

    /// <summary>
    /// Tints a material red
    /// </summary>
    /// <param name="target"></param>
    public void TintRedPulse(GameObject target)
    {
        target.GetComponent<MeshRenderer>().material.shader = litColorPulse;
        target.GetComponent<MeshRenderer>().material.SetFloat(PULSE_RED_VEC_ADDRESS, 1.0f);    // Red Value
        target.GetComponent<MeshRenderer>().material.SetFloat(PULSE_GREEN_VEC_ADDRESS, 1.6f);    // Green Value
        target.GetComponent<MeshRenderer>().material.SetFloat(PULSE_BLUE_VEC_ADDRESS, 1.6f);    // Blue Value
    }

    /// <summary>
    /// Tints a material green
    /// </summary>
    /// <param name="target"></param>
    public void TintGreenPulse(GameObject target)
    {
        target.GetComponent<MeshRenderer>().material.shader = litColorPulse;
        target.GetComponent<MeshRenderer>().material.SetFloat(PULSE_RED_VEC_ADDRESS, 1.4f);    // Red Value
        target.GetComponent<MeshRenderer>().material.SetFloat(PULSE_GREEN_VEC_ADDRESS, 1.0f);    // Green Value
        target.GetComponent<MeshRenderer>().material.SetFloat(PULSE_BLUE_VEC_ADDRESS, 1.4f);    // Blue Value
    }

    /// <summary>
    /// Tints a material blue
    /// </summary>
    /// <param name="target"></param>
    public void TintBluePulse(GameObject target)
    {
        target.GetComponent<MeshRenderer>().material.shader = litColorPulse;
        target.GetComponent<MeshRenderer>().material.SetFloat(PULSE_RED_VEC_ADDRESS, 1.4f);    // Red Value
        target.GetComponent<MeshRenderer>().material.SetFloat(PULSE_GREEN_VEC_ADDRESS, 1.4f);    // Green Value
        target.GetComponent<MeshRenderer>().material.SetFloat(PULSE_BLUE_VEC_ADDRESS, 1.0f);    // Blue Value
    }

    public void TintRed(GameObject target)
    {
        tintVec = new Vector3(1.0f, 1.6f, 1.6f);
        target.GetComponent<MeshRenderer>().material.shader = litColorShift;
        target.GetComponent<MeshRenderer>().material.SetVector(FLAT_VEC3_ADDRESS, tintVec);
    }

    public void TintGreen(GameObject target)
    {
        tintVec = new Vector3(1.6f, 1.0f, 1.6f);
        target.GetComponent<MeshRenderer>().material.shader = litColorShift;
        target.GetComponent<MeshRenderer>().material.SetVector(FLAT_VEC3_ADDRESS, tintVec);
    }

    public void TintBlue(GameObject target)
    {
        tintVec = new Vector3(1.6f, 1.6f, 1.0f);
        target.GetComponent<MeshRenderer>().material.shader = litColorShift;
        target.GetComponent<MeshRenderer>().material.SetVector(FLAT_VEC3_ADDRESS, tintVec);
    }
}
