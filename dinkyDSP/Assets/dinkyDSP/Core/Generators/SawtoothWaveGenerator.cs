using System;

/*
 * Gareth Williams
 * 
 * mono unit
 * 
 * Generate a Sawtooth - algorithm from Tonfall
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class SawtoothWaveGenerator : AbstractGenerator, IAudioNode
	{
		//abstract
		protected override double GenerateWave()
		{
			return m_phase * 2.0 - 1.0;
		}
	}
}

