using System;

/*
 * Gareth Williams
 * 
 * A wavetable buffers samples from an audio file and plays them back at the generators frequency
 * this pitch shifts the sound, this is more likely to be used in granular synthesis than as a straightforward
 * 'sample player' since it doesn't respect things like the original sample rate or the original pitch of the 
 * sample or whether it should one-shot or loop.
 * 
 * stereo
 * 
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class WavetableGenerator : AbstractGenerator, IAudioNode
	{
		public float[] buffer;
		public int position;
		public int channels = 2;
		int m_channel;
		
		//abstract
		protected override double GenerateWave()
		{
			if(buffer == null)
			{
				return 0;
			}
			
			if(m_channel >= channels)
			{
				m_channel = 0;	
			}
			
			double index = m_phase * (buffer.Length / channels);
			int prevIndex = (int)Math.Floor(index) * channels + m_channel;
			int nextIndex = (int)Math.Ceiling(index) * channels + m_channel;
			
			if(nextIndex >= buffer.Length)
			{
				nextIndex = buffer.Length - 1;	
			}
			
			float lerp = (buffer[nextIndex] + buffer[prevIndex]) * 0.5f;
			
			m_channel++;
			
			return lerp;
		}
	}
}

