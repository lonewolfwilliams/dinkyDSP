using System;
using System.Collections.Generic;

/*
 * Gareth Williams
 * 
 * mixes one or more inputs in stereo into an interleaved stereo output
 * 
 * stereo
 * 
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class StereoMixer : IAudioNode
	{
		public double masterOutputLevel = 1.0;
		public List<MixerChannel> m_inputs = new List<MixerChannel>();
		
		int m_channel;
		
		#region IAudioNode
		public double GetSample ()
		{
			if(m_channel >= m_inputs.Count)
			{
				m_channel = 0;	
			}
			
			double sumAmplitude = 0;
			foreach(MixerChannel mixerIn in m_inputs)
			{
				if((m_channel & 1) == 0) // left
				{
					sumAmplitude += mixerIn.inputNode.GetSample() * 
						mixerIn.pan *
						mixerIn.outputLevel;
				}
				else //right
				{
					sumAmplitude += mixerIn.inputNode.GetSample() * 
						1 - mixerIn.pan *
						mixerIn.outputLevel;
				}
			}
			
			return (sumAmplitude / m_inputs.Count) * masterOutputLevel;
		}
		#endregion
		
		//could expose the list but I want the interfaces for mix and split to be consistent
		public void AddInput(IAudioNode input)
		{
			MixerChannel mixerIn = new MixerChannel();
				
			mixerIn.inputNode = input;
			mixerIn.pan = 0.5;
			mixerIn.outputLevel = 1.0;
			
			m_inputs.Add(mixerIn);	
		}
	}
	
	public class MixerChannel
	{
		public double outputLevel;
		public double pan;
		public IAudioNode inputNode;
	}
}
