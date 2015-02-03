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

    private int m_maxStripeNum = 50;
    private List<Stripe> m_stripes;

	private void Awake()
	{
        m_stripes = new List<Stripe>();

		camera.depthTextureMode |= DepthTextureMode.DepthNormals;
        camera.depthTextureMode |= DepthTextureMode.Depth;
	}

    private void Update()
    {
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
        Stripe stripe = new Stripe();
        stripe.maxRadius = Random.Range(7.5f, 10.0f);
        stripe.speed = Random.Range(5.0f, 10.0f);
        stripe.radius = 0.0f;
        stripe.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        stripe.size = Random.Range(0.2f, 1.0f);
        stripe.fade = 1.0f;
        stripe.center = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
        m_stripes.Add(stripe);
    }

    private void SetStripesParameters()
    {
        m_material.SetFloat("_StripeNum", m_stripes.Count);
        for(int i=0; i<m_stripes.Count && i<m_maxStripeNum; ++i)
        {
            m_material.SetVector("_StripesColor" + i.ToString(), m_stripes[i].color);
            m_material.SetVector("_StripesCenter" + i.ToString(), m_stripes[i].center);
            Vector4 properties = new Vector4(m_stripes[i].radius, m_stripes[i].size, m_stripes[i].fade);
            m_material.SetVector("_Stripes" + i.ToString(), properties);
        }
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
}
