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
	public abstract class AbstractGenerator : IHasPitch
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
		
		#region IAudioNode
		public double GetSample()
		{
			m_phase += m_frequency / Driver.sampleRate;
			m_phase -= (int)m_phase;
			
			double amplitude = GenerateWave();
			
			return amplitude;
		}
		#endregion
		
		protected abstract double GenerateWave();
	}
}

