using System;

/*
 * Gareth Williams
 * 
 * Clock produces a linear control voltage of time elapsed in MS 
 * at the current sample rate.
 * 
 * mainly intended as a tool for other Modulators
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class Clock : IAudioNode, IDisposable
	{
		double m_positionInSamples = 0;
		int m_samplesPerMS = Driver.sampleRate / 1000;
		
		public void Reset()
		{
			m_positionInSamples = 0;	
		}
		
		double m_signal;
		
		public Clock()
		{
			Driver.PreSampleGenerated += (sender, e) => GenerateSignal();	
		}
		
		#region IAudioNode implementation
		public event SampleEventHandler SampleGenerated;
		
		void GenerateSignal()
		{
			m_positionInSamples++;
			m_signal = m_positionInSamples / m_samplesPerMS;
			
			if(SampleGenerated != null)
			{
				SampleGenerated(m_signal);	
			}
		}
		
		public double GetSample ()
		{
			return m_signal;	
		}
		#endregion

		#region IDisposable implementation
		public void Dispose ()
		{
			Driver.PreSampleGenerated -= (sender, e) => GenerateSignal();
		}
		#endregion
	}
}