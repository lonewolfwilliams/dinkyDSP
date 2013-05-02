
/*
 * Gareth Williams
 * 
 * Clock repeatedly outputs a control voltage of 1 at a set interval
 * and 0 at all other times
 * 
 * mainly intended as a tool for other Modulators
 * 
 * TODO: this should be a generator...
 * 
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class Clock : IAudioNode
	{
		//needs to be more granular ?
		public int tickFrequencyMS = 1000;
		
		double m_position;
		int m_samplesPerMS = Driver.sampleRate / 1000;
		
		public Clock()
		{
			m_position = m_samplesPerMS * tickFrequencyMS; //trigger on first GetSample();	
		}
		
		#region IAudioNode implementation
		public double GetSample ()
		{
			double voltage = 0;
			
			m_position++;
			if( m_position / m_samplesPerMS >= tickFrequencyMS)
			{
				voltage = 1;
				m_position = 0;
			}
			
			return voltage;
		}
		#endregion
	}
}