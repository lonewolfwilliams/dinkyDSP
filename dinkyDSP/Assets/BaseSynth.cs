using System;
using System.Collections.Generic;

/*
 * Gareth Williams
 * 
 * base synth is a wrapper class which forms the skeleton for a synthesiser nested graph segment (used by synthfactory)
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class BaseSynth : IAudioNode, IHasPitch
	{
		#region IHasPitch implementation
		public float Frequency 
		{
			get 
			{
				if(false == inputNode is IHasPitch)
				{
					return 0;	
				}
				
				return (inputNode as IHasPitch).Frequency;
			}
			set 
			{
				if(inputNode is IHasPitch)
				{
					(inputNode as IHasPitch).Frequency = value;
				}
			}
		}
		#endregion
		
		public IAudioNode inputNode
		{
			get 
			{
				return m_sequencer.InputNode;	
			}
			set
			{
				m_sequencer.InputNode = value;
			}
		}
		
		public IAudioNode outputNode
		{
			get
			{
				return m_metronome;
			}
		}
		
		Common.noteDuration m_stepDuration;
		public Common.noteDuration stepDuration
		{
			get
			{
				return m_stepDuration;
			}
			set
			{
				m_stepDuration = value;
				m_sequencer.StepLength = m_stepDuration;
				m_metronome.StepLength = m_stepDuration;
			}
		}
		
		int m_bpm;
		public int BPM
		{
			get
			{
				return m_bpm;
			}
			set
			{
				m_bpm = value;
				m_sequencer.Bpm = m_bpm;
				m_metronome.Bpm = m_bpm;
			}
		}
		
		//todo: bad practice...
		public List<StepData> Sequence
		{
			get 
			{
				return m_sequencer.sequence;	
			}
			set
			{
				m_sequencer.sequence = value;	
			}
		}
			
		Sequencer m_sequencer;
		ADSREnvelope m_envelope;
		Metronome m_metronome;
		
		public BaseSynth()
		{
			m_sequencer = new Sequencer();
			m_sequencer.Bpm = 0;
			m_sequencer.StepLength = Common.noteDuration.whole;
			m_sequencer.InputNode = inputNode;
			
			m_envelope = new ADSREnvelope();
			m_envelope.AttackMS = 10;
			m_envelope.DecayMS = 250;
			m_envelope.sustainLevel = 0;
			m_envelope.SustainMS = 0;
			m_envelope.ReleaseMS = 0;
			m_envelope.InputNode = m_sequencer;
			
			m_metronome = new Metronome();
			m_metronome.Bpm = 0;
			m_metronome.StepLength = Common.noteDuration.whole;
			m_metronome.InputNode = m_envelope;
			
		}
		
		#region IAudioNode implementation
		public double GetSample ()
		{
			if(outputNode == null)
			{
				return 0;	
			}
			
			return outputNode.GetSample();
		}
		#endregion
	}
}

