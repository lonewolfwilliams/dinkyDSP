using System;
using System.Collections.Generic;
using com.lonewolfwilliams.dinkyDSP;
/*
 * Gareth Williams
 * 
 * This is an example of how you might make a graph using dinky DSP
 */

public class DinkyDemo
{
	//SamplePlayer m_sampleLoop;
	MonoMixer m_mixer;
	LowPassFilter m_filter;
	BaseSynth m_fmStepSynth;
	Gain m_gainer;
	
	float m_resonanceHZ = 0.8f;
	int m_delayMS = 666;
	
	int m_bpm = 100;
	public int BPM
	{
		get
		{
			return m_bpm;
		}
		set
		{
			m_bpm = value;
			
			if(m_bpm <= 0)
			{
				m_bpm = 1;
			}
			
			//update bpm on devices
			m_fmStepSynth.BPM = m_bpm;
			
			/*
				m_sampleLoop.fundamentalFrequency = 44100;
				var samplesPerBeat = m_sampleLoop.buffer.Length / channels / com.lonewolfwilliams.musipede.Common.BEATS_IN_LOOP;
				var beatsPerSecond = (float)m_bpm / 60;
				m_sampleLoop.Frequency = samplesPerBeat * beatsPerSecond;
			*/
		}
	}
	
	float m_filterCutoffHZ = 100;
	public float FilterCutoffHZ
	{
		get
		{
			return m_filterCutoffHZ;
		}
		set
		{
			m_filterCutoffHZ = value;
			
			if(m_filterCutoffHZ > 44100)
			{
				m_filterCutoffHZ = 44100;
			}
			if(m_filterCutoffHZ < 0)
			{
				m_filterCutoffHZ = 0;	
			}
			
			m_filter.cutoffFrequency = m_filterCutoffHZ;
		}
	}
	
	float m_volumeNormalised = 1.0f;
	public float VolumeNormalised
	{
		get
		{
			return m_volumeNormalised;	
		}
		set
		{
			m_volumeNormalised = value;
			
			if(m_volumeNormalised < 0)
			{
				m_volumeNormalised = 0;	
			}
			if(m_volumeNormalised > 1)
			{
				m_volumeNormalised = 1;	
			}
			
			m_gainer.drive = m_volumeNormalised;
		}
	}
	
	List<List<StepData>> arpeggios = new List<List<StepData>>() {
		new List<StepData>(){ new StepData("C", 1), 	new StepData("G", 1), 	new StepData("D", 1) },
		new List<StepData>(){ new StepData("G", 1), 	new StepData("D", 1), 	new StepData("A", 1) },
		new List<StepData>(){ new StepData("D", 1), 	new StepData("A", 1), 	new StepData("E", 1) },
		new List<StepData>(){ new StepData("A", 1), 	new StepData("E", 1), 	new StepData("B", 1) },
		new List<StepData>(){ new StepData("E", 1), 	new StepData("B", 1), 	new StepData("F", 1) },
		new List<StepData>(){ new StepData("B", 1), 	new StepData("F#", 1), 	new StepData("C#", 1) },
		new List<StepData>(){ new StepData("F#", 1), 	new StepData("C#", 1), 	new StepData("G#", 1) },
		new List<StepData>(){ new StepData("C#", 1),	new StepData("G#", 1), 	new StepData("D#", 1) },
		new List<StepData>(){ new StepData("G#", 1), 	new StepData("D#", 1), 	new StepData("A#", 1) },
		new List<StepData>(){ new StepData("D#", 1), 	new StepData("A#", 1), 	new StepData("F", 1) },
		new List<StepData>(){ new StepData("A#", 1), 	new StepData("F", 1), 	new StepData("C", 2) },
		new List<StepData>(){ new StepData("F", 1), 	new StepData("C", 2), 	new StepData("G", 2) }
	};
	
	public DinkyDemo (Driver driver)
	{
		/*	
			var sampleBank = com.lonewolfwilliams.musipede.ModelLocator.GetModelInstance<SampleBank>() as SampleBank;
			m_sampleLoop = sampleBank.GetSamplePlayerForBuffer("vexst");
			m_sampleLoop.SampleCompleted += delegate(object sender, EventArgs e) 
			{
				messaging.DispatchMessage(com.lonewolfwilliams.musipede.Common.SAMPLE_LOOPED, this, null);
			};
		*/	
		
		UnityEngine.Debug.Log("Welcome to Dinky");
		
		Random rnd = new Random();
		SynthFactory.Voice carrier = (SynthFactory.Voice)rnd.Next(2,4);//exclude noise
		SynthFactory.Voice modulator = (SynthFactory.Voice)rnd.Next(1,4);
		
		m_fmStepSynth = SynthFactory.Create(carrier, modulator);
		m_fmStepSynth.BPM = m_bpm;
		m_fmStepSynth.stepDuration = Common.noteDuration.whole;
		
		//fifths arp triad
		//todo: clumsy clumsy clumsy
		m_fmStepSynth.Sequence.Add(new StepData("C", 1));
		m_fmStepSynth.Sequence.Add(new StepData("G", 1));
		m_fmStepSynth.Sequence.Add(new StepData("D", 1));
		
		m_fmStepSynth.Sequence = arpeggios[0];
		
		m_gainer = new Gain();
		m_gainer.drive = m_volumeNormalised;
		m_gainer.InputNode = m_fmStepSynth;

		m_filter = new LowPassFilter();
		m_filter.cutoffFrequency = m_filterCutoffHZ;
		m_filter.resonance = m_resonanceHZ;
		m_filter.InputNode = m_gainer;
		
		var delay = new SimpleDelay(m_delayMS);
		delay.InputNode = m_filter;
		
		m_mixer = new MonoMixer();
		m_mixer.AddInput(delay);
		
		driver.rootNode = m_mixer;
	}
	
	public void Transpose(int interval)
	{
		m_fmStepSynth.Sequence = arpeggios[interval];	
	}
}


