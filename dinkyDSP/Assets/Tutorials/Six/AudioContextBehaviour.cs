using UnityEngine;
using System.Collections.Generic;
using com.lonewolfwilliams.dinkyDSP;
using System;

/*
 * Tutorial six
 * 
 * This tutorial talks a little about playing back samples and sampleRate
 * 
 * Rather than synthesising, you may want to make complex sound by playing short recorded clips of audio,
 * these are called samples (since they are pre-stored vertical-slices of amplitude data)
 * 
 * more to follow...
 * 
 */

namespace TutorialSix
{
	public class AudioContextBehaviour : MonoBehaviour 
	{
		/// <summary>
		/// The clips that will be buffered and made available to the Dinky Framework as 
		/// samples buffers.
		/// </summary>
		public List<AudioClip> clips = new List<AudioClip>();
		
		SampleBank m_bank;
		Driver m_driver;
		MonoMixer m_mixer;
		
		/// <summary>
		/// When Awake is called by the Unity Engine we build our simple audio graph,
		/// and attach the root node of our graph to the driver.
		/// </summary>
		void Awake()
		{
			InitialiseSampleBank();
			InitialiseGraph();
		}
		
		/// <summary>
		/// This is called by Unity Engine after it has read the audio data from all of the 
		/// Audio sources in the scene and processed them using the built-in dsp components 
		/// (which are available in pro only). This callback is where you pass data from the 
		/// driver object at the root of your DSP graph to the Unity Engine.
		/// </summary>
		/// <param name='data'>
		/// Sample data from the audio sources in the scene.
		/// </param>
		/// <param name='channels'>
		/// The number of channels that Unity Engine is working with, typically two.
		/// </param>
		void OnAudioFilterRead(float[] data, int channels)
		{
			//pass the sample data from the driver into the Unity Engine sample buffer
			com.lonewolfwilliams.dinkyDSP.Driver.channels = channels;
			m_driver.GenerateSamples(ref data);
		}
		
		//helpers------------------------------------------------------------------------------
		
		void InitialiseSampleBank()
		{
			m_bank = new SampleBank();
			foreach(AudioClip clip in clips)
			{
				m_bank.AddSampleBufferFromAudioClip(clip.name, clip);
			}
		}
		
		void InitialiseGraph()
		{
			//context
			Driver.sampleRate = 48000;
			m_driver = new Driver();
			
			//audio nodes
			var stepSeq 		= new StepSequencer();
			var voice 			= m_bank.GetSamplePlayerForBuffer("505 arp 1");
			voice.isOneshot 	= true;
			stepSeq.sequence 	= new List<StepData>(){ 
				new StepData("C", 2), 	new StepData("G", 2), new StepData("D", 2) 
			};
			m_mixer = new MonoMixer();
			m_mixer.masterOutputLevel = 0.5;
			
			//connect our graph
			stepSeq.InputNode = voice;
			m_mixer.AddInput(stepSeq);
			m_driver.rootNode = m_mixer;	
		}
		
		//helper class-----------------------------------------------------------------------
		class StepSequencer : IAudioNode, IHasInput
		{
			#region accessors
			
			public List<StepData> sequence
			{
				get
				{
					return m_frequencyModulator.sequence;
				}
				set
				{
					m_frequencyModulator.sequence = value;
				}
			}
			
			public float BPM
			{
				get 
				{
					return m_sampleTrigger.Bpm;	
				}
				set 
				{
					m_sampleTrigger.Bpm = value;
				}
			}
			
			#endregion
			
			#region IHasInput implementation
			private IAudioNode m_inputNode;
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
			
			Metronome m_sampleTrigger;
			Sequencer m_frequencyModulator;
			
			public StepSequencer()
			{
				m_sampleTrigger = new Metronome();
				m_frequencyModulator = new Sequencer();
				
				m_sampleTrigger.SampleGenerated += 
					(sample) => 
				{
					m_frequencyModulator.GenerateSignal(sample);
				};
			}
			
			#region IAudioNode implementation
			public event SampleEventHandler SampleGenerated;
			public double GetSample ()
			{
				if( m_inputNode == null)
				{
					return 0;	
				}
				
				var frequency 	= m_frequencyModulator.GetSample();
				var trigger 	= m_sampleTrigger.GetSample();
				
				if(m_inputNode is IHasPitch)
				{
					(m_inputNode as IHasPitch).Frequency = (float)frequency;
				}
				
				if( m_inputNode is IHasPosition && 
					trigger == 1)
				{
					(m_inputNode as IHasPosition).Position = 0;
				}
				
				double audioSignal = m_inputNode.GetSample();
				if(SampleGenerated != null)
				{
					SampleGenerated(audioSignal);	
				}
				
				return audioSignal;
			}
			#endregion
		}
	}
}
