using UnityEngine;
using System.Collections;

/*
 * Gareth Williams
 * 
 * modifies parameters on the synth using mouse input
 */

public class MouseInputBehaviour : MonoBehaviour 
{	
	public AudioContextBehaviour audioContext;
	
	float m_frequency;
	int m_interval;
	
	// Update is called once per frame
	void Update() 
	{
   		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
		if (Physics.Raycast(ray, out hitData))
		{
			var x = hitData.textureCoord.x;
			var y = hitData.textureCoord.y;
			
			m_frequency = 1000 - 1000.0f * x;
			m_interval = 11 - Mathf.RoundToInt(11.0f * y);
			
			if(audioContext != null)
			{
				audioContext.Cutoff = m_frequency;
				audioContext.Transpose(m_interval);
			}
		} 
    }
	
	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 100, 20), string.Format("cutoff:{0}hz", m_frequency));
		GUI.Label(new Rect(120, 10, 100, 20), string.Format("transpose:{0}", m_interval));
	}
}
