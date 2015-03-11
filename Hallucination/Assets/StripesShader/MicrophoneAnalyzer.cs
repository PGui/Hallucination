using UnityEngine;
using System.Collections;

public class MicrophoneAnalyzer : MonoBehaviour
{
    private AudioSource m_audioSource;
    private StripesEffect m_stripes;
    private float[] m_spectrum;

    [Range(0.0004f, 0.002f)]
    public float m_threshold = 0.0008f;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_stripes = GetComponent<StripesEffect>();
        if(m_audioSource == null || m_stripes == null)
        {
            Debug.LogError("AudioSource and StripesPostEffect component should be attached to the camera");
        }

        m_audioSource.clip = Microphone.Start(null, true, 1, 44100);
        if(m_audioSource.clip != null)
        {
            while(Microphone.GetPosition(null) <= 0){}

            m_audioSource.loop = true;
            m_audioSource.Play();
			m_audioSource.volume = 0.0f;
        }
        else
        {
            Debug.LogError("Failed to initialize microphone");
        }
        
        m_spectrum = new float[64];
    }

	private void Update()
    {
        m_audioSource.GetSpectrumData(m_spectrum, 0, FFTWindow.BlackmanHarris);
        
        float sum = 0;
        for(int i=0; i<m_spectrum.Length; ++i)
        {
            sum += m_spectrum[i];
        }
        sum /= m_spectrum.Length;

        if(sum > m_threshold || Input.GetButtonDown("Fire1"))
        {
            m_stripes.AddStripe();
        }
	}
}
