using System;

/*
 * Gareth Williams
 * 
 * mono unit
 * 
 * Generate a sine wave - algorithm from TonFall
 */
 
namespace com.lonewolfwilliams.dinkyDSP
{
	public class SineWaveGenerator : AbstractGenerator, IAudioNode
	{	
		//abstract
		protected override double GenerateWave()
		{
			return Math.Sin( m_phase * 2.0 * Math.PI );
		}
	}
}

