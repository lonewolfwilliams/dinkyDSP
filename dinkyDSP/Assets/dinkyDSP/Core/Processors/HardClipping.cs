using System;

/*
 * Gareth williams
 * 
 * hard clipping simply limits the amplitude to a set level to achieve a digital distortion effect
 * 
 * mono
 * 
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class HardClipping : IAudioNode, IHasInput
	{
		public double threshold = 0.2;
		
		#region IHasInput
		IAudioNode m_inputNode;
		public IAudioNode InputNode
		{
			get
			{
				return m_inputNode;
			}
			set
			{
				m_inputNode = value;
			}
		}
		#endregion
		
		#region IAudioNode implementation
		public double GetSample ()
		{
			if(m_inputNode == null)
			{
				return 0;
			}
			
			double amplitude = m_inputNode.GetSample();
			
			if(amplitude > threshold)
			{
				amplitude = threshold;	
			}
			
			return amplitude;
		}
		#endregion
		
	}
}

