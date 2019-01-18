using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class DayNightCycle : MonoBehaviour
{
    /* This script handles the transformation from day to night by changing the
     * light source's intensity and color
     * the light source's volumetric lighting color
     * the light source's volumetric light density
     * the exposure and rotation of the skybox material
     */

    [SerializeField]
    private Light sun;

    [SerializeField]
    private Gradient colorGradient, colorGradientVL, postProcessingGradient, fogColorGradient;

    [SerializeField]
    private AnimationCurve intensityCurve, skyboxExposureCurve, volumetricCurve, fogCurve, ambientLight/*, colorgradingCurveR, colorgradingCurveG, colorgradingCurveB*/;

    [SerializeField]
    private Material skybox;

    [SerializeField]
    private GameObject firstpersoncharacter;
    PostProcessingProfile postprocessingProfile;

    ColorGradingModel.Settings colmod;


    private float rotation;
    private float exposure = 0.6f;
    private float gameTime = 1800f; //1800 seconds = 30 minutes; 780 seconds = 13 minutes; use 10f for testing
    private HxVolumetricLight sunVL;

    // Use this for initialization
    void Start()
    {
        sun.intensity = 1f;
        sun.transform.eulerAngles = new Vector3(20, 90, 0);
        sunVL = sun.gameObject.GetComponent<HxVolumetricLight>();
        rotation = skybox.GetFloat("_Rotation");

        postprocessingProfile = firstpersoncharacter.GetComponent<PostProcessingBehaviour>().profile;
        // Debug.Log("[PPP]" + postprocessingProfile);

        StartCoroutine(CycleCoroutine());
    }

    IEnumerator CycleCoroutine()
    { //could allow for more than text fading
      // yield return new WaitForSeconds(2.5f);
        for (float f = 0f; f < 1f; f += 1 / gameTime * Time.deltaTime)
        { //f += 0.0001f
            sun.color = colorGradient.Evaluate(f); //not it
            sunVL.Color = colorGradientVL.Evaluate(f) * 0.5f; //not it
            sun.intensity = intensityCurve.Evaluate(f);
            sunVL.ExtraDensity = volumetricCurve.Evaluate(f);

          //  Invoke("test", 1f);

            //colmod = postprocessingProfile.colorGrading.settings;

            //colmod.curves.blue.curve = colorgradingCurveR;
            //colmod.curves.green.curve = colorgradingCurveG;
            //colmod.curves.red.curve = colorgradingCurveB;

            //sun.transform.Rotate(new Vector3 (f, 0f, 0f));
            sun.transform.eulerAngles = new Vector3(20 + 140 * f, 90, 0);

            rotation += 0.01f;
            skybox.SetFloat("_Rotation", rotation); //causes lag?
            skybox.SetFloat("_Exposure", skyboxExposureCurve.Evaluate(f)); //causes lag?
            //yield return new WaitForSeconds(0.2f); //causes lag?, was 0.2f

            //handle the fog density
            RenderSettings.fogDensity = fogCurve.Evaluate(f);
            RenderSettings.fogColor = fogColorGradient.Evaluate(f);

            RenderSettings.ambientIntensity = ambientLight.Evaluate(f);

            yield return null;
        }
    }

    public float GetGameDuration()
    {
        return gameTime;
    }
}
