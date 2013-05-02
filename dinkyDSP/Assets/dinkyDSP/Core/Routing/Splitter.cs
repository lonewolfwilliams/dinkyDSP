using System;

/*
 * Gareth Williams
 * 
 * splitter holds a single sample value for as many calls to get sample as there are 
 * attached devices, meaning one device can be connected to many
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class Splitter : IAudioNode, IHasInput
	{
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
		
		int m_childCount;
		int m_currentCount;
		double m_sample;
		public void AddOutput(IHasInput child)
		{
			child.InputNode = this;
			m_childCount++;
		}
		
		#region IAudioNode implementation
		public double GetSample ()
		{
			if(m_inputNode == null)
			{
				return 0;	
			}
			
			if(m_currentCount >= m_childCount)
			{
				m_currentCount = 0;
			}
			
			if(m_currentCount == 0)
			{
				m_sample = m_inputNode.GetSample();	
			}
			
			m_currentCount++;
			
			return m_sample;
		}
		#endregion
	}
}

