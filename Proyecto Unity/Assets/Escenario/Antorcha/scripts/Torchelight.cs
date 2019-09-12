using UnityEngine;
using System.Collections;

public class Torchelight : MonoBehaviour {
	
	public GameObject TorchLight;
	public GameObject MainFlame;
	public GameObject BaseFlame;
	public GameObject Etincelles;
	public GameObject Fumee;
	public float MaxLightIntensity;
	public float IntensityLight;
	

	void Start () {
		TorchLight.GetComponent<Light>().intensity=IntensityLight;
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
        MainFlame.GetComponent<ParticleSystem>().emissionRate=IntensityLight*20f;
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
        BaseFlame.GetComponent<ParticleSystem>().emissionRate=IntensityLight*15f;
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
        Etincelles.GetComponent<ParticleSystem>().emissionRate=IntensityLight*7f;
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
        Fumee.GetComponent<ParticleSystem>().emissionRate=IntensityLight*12f;
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
    }
	

	void Update () {
		if (IntensityLight<0) IntensityLight=0;
		if (IntensityLight>MaxLightIntensity) IntensityLight=MaxLightIntensity;		

		TorchLight.GetComponent<Light>().intensity=IntensityLight/2f+Mathf.Lerp(IntensityLight-0.1f,IntensityLight+0.1f,Mathf.Cos(Time.time*30));

		TorchLight.GetComponent<Light>().color=new Color(Mathf.Min(IntensityLight/1.5f,1f),Mathf.Min(IntensityLight/2f,1f),0f);
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
        MainFlame.GetComponent<ParticleSystem>().emissionRate=IntensityLight*20f;
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
        BaseFlame.GetComponent<ParticleSystem>().emissionRate=IntensityLight*15f;
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
        Etincelles.GetComponent<ParticleSystem>().emissionRate=IntensityLight*7f;
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos
#pragma warning disable CS0618 // El tipo o el miembro están obsoletos
        Fumee.GetComponent<ParticleSystem>().emissionRate=IntensityLight*12f;
#pragma warning restore CS0618 // El tipo o el miembro están obsoletos

    }
}
