using System;
using System.Collections.Generic;

/*
 * Gareth Williams
 * 
 * mixes one or more inputs in to a single output
 * 
 * mono
 * 
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class MonoMixer : IAudioNode
	{
		public double masterOutputLevel = 1.0;
		public List<IAudioNode> m_inputs = new List<IAudioNode>();
		
		#region IAudioNode
		public double GetSample ()
		{
			double sumAmplitude = 0;
			foreach(IAudioNode input in m_inputs)
			{
				sumAmplitude += input.GetSample();
			}
			
			var sample = (sumAmplitude / m_inputs.Count) * masterOutputLevel;
			
#if SERVICE_MODE
			if(sample > 1)
			{
				UnityEngine.Debug.LogWarning(sumAmplitude + " " + m_inputs.Count);	
			}
#endif
			return sample;
		}
		#endregion
		
		//could expose the list but I want the interfaces for mix and split to be consistent
		public void AddInput(IAudioNode input)
		{
			m_inputs.Add(input);	
		}
	}
}

