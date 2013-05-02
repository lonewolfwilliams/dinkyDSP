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
		public double GetSample ()
		{
			if(m_inputNode == null)
			{
				return 0;
			}
			
			return m_inputNode.GetSample() * drive;
		}
		#endregion
		
	}
}

