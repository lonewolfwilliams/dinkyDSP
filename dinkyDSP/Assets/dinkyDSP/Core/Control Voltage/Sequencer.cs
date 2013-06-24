using System;
using System.Collections.Generic;

/*
 * Gareth Williams
 * 
 * outputs stored note frequencies at a given tempo dictated by metronome
 * 
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class Sequencer : IAudioNode
	{
		#region accessors
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

		public List<StepData> sequence = new List<StepData>();
		
		int m_step;
		float m_frequency;
		
		#region IAudioNode implementation
		public event SampleEventHandler SampleGenerated;
		
		public void GenerateSignal(double sampleIn)
		{
			if(sequence.Count == 0)
			{
				m_frequency = 0;	
				return;
			}
			
			if(sampleIn >= 1)
			{
				if(m_step >= sequence.Count)
				{
					m_step = 0;	
				}
				
				bool isNote = Common.noteToFrequency.TryGetValue(sequence[m_step].note, out m_frequency);
				if(	true == isNote )
				{
					int octave = sequence[m_step].octave;
					m_frequency *= octave;
				}
				
				if(	sequence[m_step].note == Common.rest )
				{
					m_frequency = 0;
				}
				
				m_step++;
			}
			
			if(SampleGenerated != null)
			{
				SampleGenerated(m_frequency);	
			}	
		}
		
		public double GetSample ()
		{
			return m_frequency;
		}
		#endregion
	}
}

