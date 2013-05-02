using System;

/*
 * Gareth Williams
 * 
 * mono unit
 * 
 * Generate a pulse wave - algorithm from TonFall
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class PulseWaveGenerator : AbstractGenerator, IAudioNode
	{
		public double pulseWidth = 0.5; //square wave
		//abstract
		protected override double GenerateWave()
		{
			double amplitude = 0;
			if(m_phase < pulseWidth)
			{
				amplitude = 1.0;
			}
			else 
			{
				amplitude = -1.0;	
			}
			
			return amplitude;
		}
	}
}

