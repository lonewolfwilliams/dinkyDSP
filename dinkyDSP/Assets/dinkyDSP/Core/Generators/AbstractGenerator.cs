using System;

/*
 * Gareth Williams
 * 
 * Generators are instances of IAudioNode that generate a signal
 * 
 * phase implementation from TonFall
 * 
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public abstract class AbstractGenerator : IHasPitch, IDisposable
	{
		#region IHasPitch
		float m_frequency = Common.noteToFrequency["C"] * 3;//middle c (test tone)
		public float Frequency
		{
			get 
			{
				return m_frequency;
			}
			set
			{
				m_frequency = value;
			}
		}
		#endregion
		
		protected double m_phase;
		protected double m_amplitude;
		
		public AbstractGenerator()
		{
			Driver.PreSampleGenerated += (sender, e) => GenerateSample();	
		}
		
		void GenerateSample()
		{
			m_phase += m_frequency / Driver.sampleRate;
			m_phase -= (int)m_phase;
			
			m_amplitude = GenerateWave();
			
			if(SampleGenerated != null)
			{
				SampleGenerated(m_amplitude);
			}
		}
		
		#region IAudioNode
		public event SampleEventHandler SampleGenerated;
		public double GetSample()
		{
			return m_amplitude;
		}
		#endregion
		
		protected abstract double GenerateWave();
		
		#region IDisposable
		public void Dispose()
		{
			Driver.PreSampleGenerated -= (sender, e) => GenerateSample();	
		}
		#endregion
	}
}
