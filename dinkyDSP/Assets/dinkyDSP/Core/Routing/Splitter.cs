using System;

/*
 * Gareth Williams
 * 
 * splitter holds a single sample value for as many calls to get sample as there are 
 * attached devices, meaning one device can be connected to many
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class Splitter : IAudioNode, IHasInput, IDisposable
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
				var previousNode = m_inputNode;
				
				if(previousNode == value)
				{
					return;	
				}
				
				if(previousNode != null)
				{
					previousNode.SampleGenerated -= (sample) => m_sample = sample;
				}

				m_inputNode = value;
				
				if(m_inputNode != null)
				{
					m_inputNode.SampleGenerated += (sample) => m_sample = sample;
				}
			}
		}
		#endregion
		
		double m_sample;
		public void AddOutput(IHasInput child)
		{
			child.InputNode = this;
		}
		
		#region IAudioNode implementation
		public event SampleEventHandler SampleGenerated;
		public double GetSample ()
		{
			if(SampleGenerated != null)
			{
				SampleGenerated(m_sample);	
			}
			
			return m_sample;
		}
		#endregion

		#region IDisposable implementation
		public void Dispose ()
		{
			if(m_inputNode != null)
			{
				m_inputNode.SampleGenerated -= (sample) => m_sample = sample;
			}
		}
		#endregion
	}
}

