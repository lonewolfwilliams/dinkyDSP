using System;

/*
 * Gareth Williams
 * 
 * mono
 * 
 * very simple delay - implementation from Tonfall
 * 
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class SimpleDelay : IAudioNode, IHasInput
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
		
		double[] m_buffer;
		int m_bufferIndex;
		int m_bufferSize;
		
		double m_wet = 0.3;
		double m_dry = 0.6;
		double m_feedback = 0.4;
		
		public SimpleDelay(int millis)
		{
			m_bufferSize = (int)Math.Round(millis / 1000.0 * Driver.sampleRate);

			m_buffer = new double[m_bufferSize];
			m_bufferIndex = 0;
		}
		
		#region IAudioNode
		public event SampleEventHandler SampleGenerated;
		public double GetSample()
		{
			if(m_inputNode == null)
			{
				return 0;
			}
			
			//circular buffer
			if (m_bufferIndex >= m_bufferSize)
			{
				m_bufferIndex = 0;	
			}
			
			double inp = m_inputNode.GetSample();
			double output = 0;
			
			// READ FROM DELAY BUFFER
			double readL = m_buffer[m_bufferIndex];
			
			// WRITE INPUT TO DELAY BUFFER
			m_buffer[m_bufferIndex] = inp + readL * m_feedback;
			
			// MIX INPUT AND DELAY TO OUTPUT
			output = inp * m_dry + m_buffer[m_bufferIndex] * m_wet;
			
			m_bufferIndex++;
			
			if(SampleGenerated != null)
			{
				SampleGenerated(output);
			}
			return output;
		}
		#endregion
	}
}

