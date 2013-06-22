
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
	public class Clock : IAudioNode
	{
		double m_positionInSamples = 0;
		int m_samplesPerMS = Driver.sampleRate / 1000;
		
		public void Reset()
		{
			m_positionInSamples = 0;	
		}
		
		#region IAudioNode implementation
		public double GetSample ()
		{
			m_positionInSamples++;
			double voltage = m_positionInSamples / m_samplesPerMS;
			
			return voltage;
		}
		#endregion
	}
}