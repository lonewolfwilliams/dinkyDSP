using System;
using System.Collections.Generic;

/*
 * Gareth Williams
 * 
 * a metronome is a clock with musical timing - it controls the position of an IHasPosition
 * best used with adsr envelope
 * 
 * think of it as the rhythmic partner of the metronome
 * 
 * this is an example of a basic composite node
 * 
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class Metronome : IAudioNode, IHasInput
	{
		#region accessors
		Common.noteDuration m_stepLength;
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
		
		float m_bpm;
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

		#region IHasInput implementation
		IAudioNode m_inputNode;
		public IAudioNode InputNode 
		{
			get 
			{
				return m_inputNode;
			}
			set 
			{
				m_inputNode = value;
			}
		}
		#endregion
		
		Clock m_clock = new Clock();
		int m_step;
		
		#region IAudioNode implementation
		public double GetSample ()
		{
			if( m_inputNode == null)
			{
				return 0;	
			}
			
			if( m_clock.GetSample() == 1 &&
				m_inputNode is IHasPosition)
			{
				(m_inputNode as IHasPosition).Position = 0;	
			}
			
			double outSample = m_inputNode.GetSample();
			
			if(outSample > 1)
			{
#if SERVICE_MODE
				UnityEngine.Debug.LogWarning("clipping in metro " + outSample);	
#endif
			}
			
			return outSample;
		}
		#endregion
		
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
			
			m_clock.tickFrequencyMS = (int)Math.Floor(durationMS);
		}
	}
}

