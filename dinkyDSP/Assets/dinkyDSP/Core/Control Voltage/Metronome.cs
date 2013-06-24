using System;
using System.Collections.Generic;

/*
 * Gareth Williams
 * 
 * a metronome is a clock with musical timing - it controls the position of an IHasPosition
 * best used with adsr envelope
 * 
 * think of it as the rhythmic partner clock
 * 
 * this is an example of a basic composite node
 * 
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class Metronome : IAudioNode, IDisposable
	{
		#region accessors
		Common.noteDuration m_stepLength = Common.noteDuration.whole;
		public Common.noteDuration StepLength
		{
			get 
			{
				return m_stepLength;	
			}
			set
			{
				m_stepLength = value;
				calculateFrequency();
			}
		}
		
		float m_bpm = 100;
		public float Bpm
		{
			get
			{
				return m_bpm;	
			}
			set
			{
				m_bpm = value;
				calculateFrequency();
			}
		}
		
		#endregion

		Clock m_clock = new Clock();
		double m_signal;
		double m_tickFrequencyMS;
		int m_step;
		
		public Metronome()
		{
			calculateFrequency();	
			m_clock.SampleGenerated += (sample) => GenerateSignal(sample);
		}
	
		void calculateFrequency()
		{
			float msPerBeat = (1 / (m_bpm / 60)) * 1000;
			
			float durationMS = msPerBeat;
			switch(m_stepLength)
			{
				case Common.noteDuration.whole :
					durationMS = msPerBeat;
				break;
				case Common.noteDuration.half :
					durationMS = msPerBeat / 2;
				break;
				case Common.noteDuration.quarter :
					durationMS = msPerBeat / 4;
				break;
				case Common.noteDuration.eighth :
					durationMS = msPerBeat / 8;
				break;
				case Common.noteDuration.sixteenth :
					durationMS = msPerBeat / 16;
				break;
					
				case Common.noteDuration.wholeTriplet :
					durationMS = msPerBeat / 1 / 3;
				break;
				case Common.noteDuration.halfTriplet :
					durationMS = msPerBeat / 2 / 3;
				break;
				case Common.noteDuration.quarterTriplet :
					durationMS = msPerBeat / 4 / 3;
				break;
				case Common.noteDuration.eightTriplet :
					durationMS = msPerBeat / 8 / 3;
				break;
				case Common.noteDuration.sixteenthTriplet :
					durationMS = msPerBeat / 16 / 3;
				break;
				
			}
			
			m_tickFrequencyMS = (int)Math.Floor(durationMS);
		}
		
		#region IAudioNode implementation
		public event SampleEventHandler SampleGenerated;
		
		void GenerateSignal(double sampleIn)
		{
			m_signal = 0;
			if( sampleIn >= m_tickFrequencyMS )
			{
				m_signal = 1;
				m_clock.Reset();
			}
			
			if(SampleGenerated != null)
			{
				SampleGenerated(m_signal);	
			}
		}
		
		public double GetSample ()
		{
			return m_signal;
		}
		#endregion

		#region IDisposable implementation
		public void Dispose ()
		{
			m_clock.SampleGenerated -= (sample) => GenerateSignal(sample);
		}
		#endregion
	}
}

