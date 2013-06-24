using System;

/*
 * Gareth Williams
 * 
 * simple gain / amplifier
 * 
 * mono
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class Gain : IAudioNode, IHasInput
	{
		public double drive = 1.0;
		
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
		public event SampleEventHandler SampleGenerated;
		public double GetSample ()
		{
			if(m_inputNode == null)
			{
				return 0;
			}
			
			double sampleOut = m_inputNode.GetSample() * drive;
			if(SampleGenerated != null)
			{
				SampleGenerated(sampleOut);
			}
			return sampleOut;
		}
		#endregion
		
	}
}

