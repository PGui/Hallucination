using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StripesEffect : MonoBehaviour
{
    public class Stripe
    {
        public float maxRadius;
        public float speed;
        public float radius;
        public Color color;
        public float size;
        public float fade;
        public Vector3 center;
    }

	public Material m_material;

	//Never make it larger than 30 (texture size issue with D3D9 rendering)
    private int m_maxStripeNum = 30;
    private List<Stripe> m_stripes;
    private Texture2D m_dataTex;

	private float m_lastStripe = 0.0f;
	private float m_stripeInterval = 0.3f;

	private vp_FPPlayerEventHandler m_playerController;

	private void Awake()
	{
		m_playerController = transform.parent.GetComponent<vp_FPPlayerEventHandler>();

        m_stripes = new List<Stripe>();

		camera.depthTextureMode |= DepthTextureMode.DepthNormals;
        camera.depthTextureMode |= DepthTextureMode.Depth;

        m_dataTex = new Texture2D(256, 1);
        m_dataTex.filterMode = FilterMode.Point;
        m_dataTex.anisoLevel = 1;
        m_dataTex.mipMapBias = 0;
        m_dataTex.Apply();
	}

    private void Update()
    {
		Vector3 playerVel = m_playerController.Velocity.Get();
		if(playerVel.magnitude > 1.0f)
		{
			AddStripe();
		}

        int listCount = m_stripes.Count;
        for(int i=0; i<listCount; ++i)
        {
            m_stripes[i].radius += Time.deltaTime * m_stripes[i].speed;
            if(m_stripes[i].radius > m_stripes[i].maxRadius)
            {
                m_stripes[i].fade -= Time.deltaTime;
                if(m_stripes[i].fade < 0.0f)
                { 
                    m_stripes.Remove(m_stripes[i]);
                    i--;
                    listCount--;
                }
            }

        }
    }

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
        SetCameraParameters();
        SetStripesParameters();
		CustomGraphicsBlit(source, destination);
	}

    public void AddStripe()
    {
		if(m_stripes.Count >= m_maxStripeNum ||
		   Time.time - m_lastStripe <= m_stripeInterval)
		{
			return;
		}

        Stripe stripe = new Stripe();
        stripe.maxRadius = Random.Range(25.0f, 30.0f);
        stripe.speed = Random.Range(5.0f, 10.0f);
        stripe.radius = 0.0f;
        stripe.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        stripe.size = Random.Range(0.2f, 1.0f);
        stripe.fade = 1.0f;
        stripe.center = transform.position + new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
        m_stripes.Add(stripe);

		m_lastStripe = Time.time;
    }

    private void SetStripesParameters()
    {
		m_material.SetFloat("_StripeNum", m_stripes.Count);
		
        for(int i=0; i<m_stripes.Count && i<m_maxStripeNum; i++)
        {
			//Color
            m_dataTex.SetPixel(i*7, 0, m_stripes[i].color);

			//Radius
			Color radius = FloatToRGBA(m_stripes[i].radius);
			m_dataTex.SetPixel(i*7 + 1, 0, radius);

			//Size
			Color size = FloatToRGBA(m_stripes[i].size);
			m_dataTex.SetPixel(i*7 + 2, 0, size);

			//Fade
			Color fade = FloatToRGBA(m_stripes[i].fade);
			m_dataTex.SetPixel(i*7 + 3, 0, fade);

			//Center
			Color centerX = FloatToRGBA(m_stripes[i].center.x);
			Color centerY = FloatToRGBA(m_stripes[i].center.y);
			Color centerZ = FloatToRGBA(m_stripes[i].center.z);
			m_dataTex.SetPixel(i*7 + 4, 0, centerX);
			m_dataTex.SetPixel(i*7 + 5, 0, centerY);
			m_dataTex.SetPixel(i*7 + 6, 0, centerZ);
        }
        m_dataTex.Apply();
		
        m_material.SetFloat("_TexGranularity", 1.0f / 256.0f);
        m_material.SetTexture("_DataTex", m_dataTex);
    }

    private void SetCameraParameters()
    {
        float near = camera.nearClipPlane;
		float far = camera.farClipPlane;
		float fov = camera.fieldOfView;
		float aspect = camera.aspect;

		Vector3 toRight = camera.transform.right * near * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad) * aspect;
		Vector3 toTop = camera.transform.up * near * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);

		Vector3 topLeft = camera.transform.forward * near - toRight + toTop;
		float scale = topLeft.magnitude * far / near;
		topLeft.Normalize();
		topLeft *= scale;

		Vector3 topRight = camera.transform.forward * near + toRight + toTop;
		topRight.Normalize();
		topRight *= scale;

		Vector3 bottomRight = camera.transform.forward * near + toRight - toTop;
		bottomRight.Normalize();
		bottomRight *= scale;

		Vector3 bottomLeft = camera.transform.forward * near - toRight - toTop;
		bottomLeft.Normalize();
		bottomLeft *= scale;

		Matrix4x4 frustumCorners = new Matrix4x4();
		frustumCorners.SetRow(0, topLeft);
		frustumCorners.SetRow(1, topRight);
		frustumCorners.SetRow(2, bottomRight);
		frustumCorners.SetRow(3, bottomLeft);

		m_material.SetMatrix("_FrustumCornersWPos", frustumCorners);
		m_material.SetVector("_CameraWPos", camera.transform.position);
    }

	private void CustomGraphicsBlit(RenderTexture source, RenderTexture destination)
	{
		RenderTexture.active = destination;	
		m_material.SetTexture("_MainTex", source);

		GL.PushMatrix();
		GL.LoadOrtho();

		m_material.SetPass(0);

		GL.Begin(GL.QUADS);
		
		GL.MultiTexCoord2(0, 0.0f, 0.0f); 
		GL.Vertex3(0.0f, 0.0f, 3.0f);
		
		GL.MultiTexCoord2(0, 1.0f, 0.0f); 
		GL.Vertex3(1.0f, 0.0f, 2.0f);
		
		GL.MultiTexCoord2(0, 1.0f, 1.0f); 
		GL.Vertex3(1.0f, 1.0f, 1.0f);
		
		GL.MultiTexCoord2(0, 0.0f, 1.0f); 
		GL.Vertex3(0.0f, 1.0f, 0.0f);
		
		GL.End();
		GL.PopMatrix();
	}

	//Encodes floats in range [-5000; 5000]
	private Vector4 FloatToRGBA(float v)
    {
		v = v/10000.0f + 0.5f;
        Vector4 enc = new Vector4(1.0f, 255.0f, 65025.0f, 160581375.0f);
        enc *= v;

        //Frac function
        enc.x = enc.x - (int) enc.x;
        enc.y = enc.y - (int) enc.y;
        enc.z = enc.z - (int) enc.z;
        enc.w = enc.w - (int) enc.w;

        enc.x -= enc.y * 1.0f/255.0f;
        enc.y -= enc.z * 1.0f/255.0f;
        enc.z -= enc.w * 1.0f/255.0f;

        return enc;
    }
}
