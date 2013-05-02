using System;
using System.Collections.Generic;

/*
 * Gareth Williams
 * 
 * a sequencer controls the frequency of an IhasPitch to set notes
 * at a specified musical timing.
 * 
 * think of it as the melodic partner of the metronome
 * 
 * this is an example of a basic composite node
 * 
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class Sequencer : IAudioNode, IHasInput
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
		
		public int CurrentStep
		{
			get
			{
				//if the step has been incremented at the end of getSample
				//but not yet reset at the start it will need rolling over...
				int currentStep = m_step;
				if(currentStep >= sequence.Count)
				{
					currentStep = 0;	
				}
				return currentStep;
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
		
		public List<StepData> sequence = new List<StepData>();
		
		Clock m_clock = new Clock();
		int m_step;
		
		#region IAudioNode implementation
		public double GetSample ()
		{
			if(m_inputNode == null)
			{
				return 0;	
			}
			
			if(m_clock.GetSample() == 1)
			{
				if(m_step >= sequence.Count)
				{
					m_step = 0;	
				}
				
				if( m_inputNode is IHasPosition)
				{
					(m_inputNode as IHasPosition).Position = 0;	
				}
				
				float frequency = 0;
				bool isNote = Common.noteToFrequency.TryGetValue(sequence[m_step].note, out frequency);
				if(	true == isNote && 
					m_inputNode is IHasPitch )
				{
					int octave = sequence[m_step].octave;
					(m_inputNode as IHasPitch).Frequency = frequency * octave;
				}
				
				if(	sequence[m_step].note == Common.rest &&
					m_inputNode is IHasPitch )
				{
					(m_inputNode as IHasPitch).Frequency = 0;
				}
				
				m_step++;
			}
			
			double outSample = m_inputNode.GetSample();
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
				//regular
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
					
				//triplet
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

