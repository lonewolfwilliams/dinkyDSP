using System;

/*
 * Gareth Williams
 * 
 * mono unit
 * 
 * Generate white noise
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class NoiseWaveGenerator : AbstractGenerator, IAudioNode
	{
		Random rnd = new Random();
		//abstract
		protected override double GenerateWave()
		{
			return rnd.NextDouble();
		}
	}
}

