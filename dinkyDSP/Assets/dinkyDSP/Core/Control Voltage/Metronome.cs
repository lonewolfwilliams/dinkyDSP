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
	public class Metronome : IAudioNode
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
				resetClock();
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
				resetClock();
			}
		}
		
		#endregion

		Clock m_clock = new Clock();
		double m_tickFrequencyMS;
		int m_step;
		
		#region IAudioNode implementation
		public double GetSample ()
		{
			double outSample = 0;
			if( m_clock.GetSample() >= m_tickFrequencyMS )
			{
				outSample = 1;
				m_clock.Reset();
			}
			
			return outSample;
		}
		#endregion
		
		public Metronome()
		{
			resetClock();	
		}
		
		void resetClock()
		{
			if(m_clock == null)
			{
				return;	
			}
			
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
	}
}

