using System;

/*
 * Gareth Williams
 * 
 * similar to a wavetable except that it corrects for fundamental pitch, stereo channels and 
 * source sample rate
 * 
 * this should be used to playback pitched samples ala modtrackers
 * 
 * mono
 * 
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class SamplePlayer : IAudioNode, IHasPitch, IHasPosition
	{
		public event EventHandler SampleCompleted;
		
		public int originalSampleRate = 44100;
		public float fundamentalFrequency = 261.63f;
		public bool isOneshot = false;
		public float[] buffer;
		
		#region IPitchable
		protected double m_position;
		public double Position
		{
			get
			{
				return m_position;
			}
			set
			{
				m_position = value;
			}
		}
		#endregion
		
		#region IPitchable
		protected float m_frequency;
		public float Frequency
		{
			get
			{
				return m_frequency;
			}
			set
			{
				m_frequency = value;
				recalculate();
			}
		}
		#endregion
		
		double m_increment;
		int m_channel;
		
		#region IAudioNode implementation
		public double GetSample ()
		{
			if(buffer == null)
			{
				return 0;
			}
			
			if(	m_position >= buffer.Length &&
				isOneshot)
			{
				return 0;	
			}
			
			if(m_position >= buffer.Length)
			{
				m_position = 0;
				
				if(SampleCompleted != null)
				{
					SampleCompleted(this, EventArgs.Empty);	
				}
			}
			
			//get 'block' position within higher samplerate
			int prevIndex = (int)Math.Floor(m_position);// * channels + m_channel;
			int nextIndex = (int)Math.Ceiling(m_position);// * channels + m_channel;
			
			if(nextIndex >= buffer.Length)
			{
				nextIndex = buffer.Length - 1;	
			}
			
			//smooth and downsample for current 'sub-position' in 'block'
			double sampleDelta = buffer[nextIndex] - buffer[prevIndex];
			double interpolationAmount = m_position - prevIndex;
			double lerp = buffer[prevIndex] + sampleDelta * interpolationAmount;
			
			m_position += m_increment;
			
			return lerp;
		}
		#endregion
		
		protected void recalculate()
		{
			double scale = (double)originalSampleRate / Driver.sampleRate;
			float pitchShift = m_frequency / fundamentalFrequency;
			m_increment = scale * pitchShift;
		}
	}
}

