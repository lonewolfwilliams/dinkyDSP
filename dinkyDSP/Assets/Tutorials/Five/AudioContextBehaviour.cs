using UnityEngine;
using System.Collections.Generic;
using com.lonewolfwilliams.dinkyDSP;
using System;

/*
 * Tutorial five
 * 
 * This tutorial shows how you might use Dinky for a more practical purpose, and introduces control voltage
 * audionodes.
 * 
 * So, all that DSP Mambo Jimbo is great you think to yourself, but what can I actually use Dinky for? 
 * Stop running around the houses in your little car!
 * 
 * As you can see the custom Audio node in this patch has grown somewhat bigger, it also illustrates the flexibility
 * of the Dinky Framework - you can nest Audio nodes inside other audio nodes to make more complex units - in 
 * this case a raygun synthesiser!
 * 
 * Unfortunately this graph also highlights one of the drawbacks of the framework, which is that for something simple
 * Dinky can be overkill. The same could be achieved with an AudioSource and some randomisation in a scripted component,
 * however, examples such as this are only the beginning! If you have bigger ideas the framework will help you realise them.
 * 
 * Let's look at some of the nested graph in our custom audio node.
 * 
 * The first thing to note is that we have introduced some new audio nodes, these come from Dinky's Control Voltage namespace,
 * as with the generators, these nodes produce waveforms, which we access one sample at a time through GetSample. However,
 * unlike the generators, the intentional use for control voltages are to modulate other parameters, in this case
 * to apply an attack sustain decay release envelope to a sawtooth wave.
 * 
 * The ADSR envelope requires a control voltage of it's own to set the position in time, that we want to retrieve
 * the corresponding amplitude value of the envelope, for. This control voltage signal comes from the clock, that produces
 * a linearly increasing value in milliseconds that corresponds to the position in time for the number of samples processed.
 * 
 * Note that it is our responsibility to reset the clock so that the ADSR envelope retriggers when we press the button.
 *
 * An important point to grasp is that whilst the envelope shapes the amplitude of the sound in response to button presses, 
 * sound is continually being synthesised by the graph, even if it is outputting zero's (as all audio units in a state of rest should do).
 * 
 * What to do next ?
 * 
 * You could try to make the raygun GetSample method more efficient by bypassing calculations in between button presses - 
 * remember! the graph must run continously even if it is outputting silence!
 * 
 * The connections in Dinky are very flexible, perhaps you should experiment with say using a sine wave generator
 * to modulate the freqeuncy of another wave generator (Frequency Modulation), or perhaps modulate the amplitude of the samples output by 
 * that generator (Amplitude Modulation)? You may be surprised by the new tones you can generate using these techniques :)
 * 
 */

namespace TutorialFive
{
	public class AudioContextBehaviour : MonoBehaviour 
	{
		Driver m_driver;
		MonoMixer m_mixer;
		RayGun m_raygun;
		
		/// <summary>
		/// When Awake is called by the Unity Engine we build our simple audio graph,
		/// and attach the root node of our graph to the driver.
		/// </summary>
		void Awake()
		{
			//context
			Driver.sampleRate = 48000;
			m_driver = new Driver();
			
			//audio nodes
			m_raygun = new RayGun();
			m_mixer = new MonoMixer();
			m_mixer.masterOutputLevel = 0.5;
			
			//connect our graph
			m_mixer.AddInput(m_raygun);
			m_driver.rootNode = m_mixer;
		}
		
		void OnGUI()
		{
			GUI.enabled = m_raygun.GetGunIsReady();
			if(GUI.Button(new Rect(10,10,100,100), "Fire ray gun"))
			{
				m_raygun.TryFireGun();
			}
			
			GUI.Label(new Rect(10, 110, 100, 50), "alien invasion simulator v 0.1");
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
		
		//helper classes---------------------------------------------------------------
		
		/// <summary>
		/// Our first nested graph element :) It creates a small graph for making a ray gun noise with 
		/// some randomised characteristics.
		/// </summary>
		class RayGun : IAudioNode
		{
			SawtoothWaveGenerator m_sawTooth;
			Gain m_gainer;
			Clock m_position;
			ADSREnvelope m_envelope;
			IAudioNode m_nestedRootNode;
			
			public RayGun()
			{
				//audio nodes
				m_sawTooth = new SawtoothWaveGenerator();
				m_position = new Clock();
				m_envelope = new ADSREnvelope();
				m_envelope.sustainLevel = 0f; // decay should fade to sustain of 0
				m_envelope.AttackMS = 10; //fade in duration (a little fade in to prevent 'clicks')
				m_envelope.DecayMS = 1000; //fade out duration
				
				m_gainer = new Gain();
				m_gainer.drive = 1.0;
				
				//connect our nested graph
				//note how the sample output from the clock can be used to control the envelope position
				m_position.SampleGenerated += (sample) => m_envelope.Position = m_position.GetSample();
				m_gainer.InputNode = m_sawTooth;
			}
			
			public void TryFireGun()
			{
				if(false == GetGunIsReady())
				{
					Debug.Log("gun is not ready");
					return;	
				}
				
				randomiseParameters();
				m_position.Reset();
			}
			
			public bool GetGunIsReady()
			{
				return m_envelope.Position > m_envelope.lengthMS;
			}
			
			/// <summary>
			/// Randomise some of the values of the graph to get variation in our raygun sound
			/// </summary>
			void randomiseParameters()
			{
				//this should look familiar:
				var lowOctave = Common.noteToFrequency["C"] * 7;
				var highOctave = Common.noteToFrequency["C"] * 9;
				m_sawTooth.Frequency = UnityEngine.Random.Range(lowOctave, highOctave);
				
				var lowDrive = 0.3f;
				var highDrive = 0.9f; //remember no more than one!
				m_gainer.drive = UnityEngine.Random.Range(lowDrive, highDrive);
				
				var lowDecayMS = 800;
				var highDecayMS = 1000;
				m_envelope.DecayMS = UnityEngine.Random.Range(lowDecayMS, highDecayMS);
			}
			
			#region IAudioNode implementation
			//we ignore this event for now...
			public event SampleEventHandler SampleGenerated;
			public double GetSample ()
			{
				//signals don't necessarily have to be used to produce a sound - here the signal
				//is used to set the position on the x axis (time) for the y lookup into the adsr envelope
				//(amplitude)...
				var control = m_envelope.GetSample();
				
				//set the frequency from the control
				m_sawTooth.Frequency *= 0.99999f;
				
				//get the audio signal from the graph
				var audio = m_gainer.GetSample();
				
				//return the sample scaled by the amplitude from the envelope
				return audio * control;
			}
			#endregion
			
		}
	}
}
