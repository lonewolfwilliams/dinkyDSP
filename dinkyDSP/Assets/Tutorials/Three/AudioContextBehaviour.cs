using UnityEngine;
using System.Collections.Generic;
using com.lonewolfwilliams.dinkyDSP;
using System;

/*
 * Tutorial three
 * 
 * This tutorial shows how you might make your own audio node, and explains a little more about sample data.
 * 
 * Following on from the previous tutorial:
 * we have a graph consisting of a Sinewave generator 
 * wired into a gain unit (this increases the amplitude of the signal)
 * wired into a custom audio node (this limits the volume of the signal)
 * wired into a mixer that is defined as the root node of the graph on the driver. 
 * The OnAudioFilterRead
 * callback transfers the sample data generated from the graph by the driver into the sample buffer 
 * of the Unity Engine.
 * 
 * As you can see things are getting a little more complicated :S, but also a little more cool :D
 * 
 * The signal generated by the graph is represented by sample data, every time the graph is run a sample is generated
 * and passed down through each unit in the graph until it reaches the driver. In the graph in this tutorial the sample
 * is generated by the SineWaveGenerator, passed down to the gain unit, passed down to the Custom audio node, 
 * passed down to the mixer and finally passed down to the driver.
 * 
 * Each sample is a vertical 'slice' of the signals waveform, where the amplitude of the signal is 
 * the same as the value of the sample, as we know from tutorial two, if the sample value is greater than one 
 * then clipping will occur. This is not the full story however, since the amplitude represents a slice of a 
 * waveform, it can vary between -1 and +1 as the waveform crosses from a positive to a negative phase, so in practice
 * we need to ensure the signal is never greater than +1 OR less than -1.
 * 
 * With this in mind you can see how a custom audio node can be used to prevent the signal from exceeding one; 
 * we look at the incoming sample value and scale it appropriately. You may ask yourself why we don't simply
 * limit the signal (if(sample > 1){ sample = 1; }) The answer is that limiting the signal will also produce
 * clipping, because we 'cutting off' the top and bottom of the waveform when it is larger than one. This is the 
 * true meaning of clipping we are 'clipping off' the top or bottom of the waveform and giving it a new shape,
 * this shape will not be an accurate representation of the signal we want to generate. It will sound distorted.
 * 
 * Note that this time we have exposed the gain amount instead of the volume, try increasing the gain to verify 
 * that the volume limiter is working by reducing the signal without clipping.
 * 
 * Try increasing the gain to a very large value, like 17, you will notice that even though the signal isn't clipping, 
 * there will still be distortion (although no way near the distortion you would get from clipping). Why is this?
 * Well, I kindof lied to you again. If you think about what is happening to the waveform, all the vertical slices of the 
 * amplitude are getting 'taller' and we are squashing (compressing) the ones that are 'taller' than the 
 * threshold of 1. As the gain increases the amount of the slices on or around the threshold will increase, so we
 * are still changing the shape of the waveform, we are still distorting it.
 * 
 * If we want to preserve the signal we need to scale it uniformly, so we could for example, compliment our previous
 * gain unit with a drive of 1.5 with a second gain unit, that has a drive of 0.5 scaling the signal back down.
 * 
 * Note that audible distortion will only take place once the signal goes to the driver, we can scale up and scale down
 * as much as we want prior to this.
 * 
 * What to do Next ?
 *
 * Try making a distortion unit, using the VolumeLimiter class as your template, for bonus points
 * you could 'nest' the gain unit inside the custom audio node, and expose the relevant properties so that 
 * your audio node has a variable distortion amount.
 * 
 * Have I lied to you again? What do you think happens if we make a sample that exceeds the size of a double?
 * what if we stored our samples in floats instead ?
 */

namespace TutorialThree
{
	public class AudioContextBehaviour : MonoBehaviour 
	{
		public double gain = 0.5;
		
		Driver m_driver;
		SineWaveGenerator m_sineWave;
		MonoMixer m_mixer;
		Gain m_gainer;
		
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
			m_sineWave = new SineWaveGenerator();
			m_gainer = new Gain();
			m_gainer.drive = 1.5;
			var customAudioNode = new VolumeLimiter();
			m_mixer = new MonoMixer();
			m_mixer.masterOutputLevel = 0.5;
			
			//connect our graph
			m_gainer.InputNode = m_sineWave;
			customAudioNode.inputNode = m_gainer;
			m_mixer.AddInput(customAudioNode);
			m_driver.rootNode = m_mixer;
		}
		
		void Update()
		{
			//expose the gain unit's drive as a component property in the inspector
			m_gainer.drive = gain;
		}
		
		void OnGUI()
		{
			//the default frequency for SineWaveGenerator is C Octave 3 (middle C); 
			
			//so...
			
			//we define the low Octave as the frequency of C Octave 2;
			var lowOctave = Common.noteToFrequency["C"] * 2;
			//we define the high Octave as the frequency of C Octave 4;
			var highOctave = Common.noteToFrequency["C"] * 4;
			
			//and bind these values to a gui slider element
			//...and yes, I have an old laptop with a small screen...
			m_sineWave.Frequency = GUI.HorizontalSlider(
				new Rect(10, 50, 100, 50), 
				m_sineWave.Frequency, lowOctave, highOctave);
			
			GUI.Label(new Rect(10, 10, 100, 50), "ghost haunting simulator v 0.1");
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
		/// Our first custom audio node :) it scales the amplitude of the incoming sample between -1 and +1 to prevent clipping
		/// 
		/// </summary>
		class VolumeLimiter : IAudioNode
		{
			/// <summary>
			/// The audio node that we are going to process the sample data from (in this case the gain unit)
			/// the IAudioNode interface provides a 'contract' that 'guarantees' that we can 
			/// 'GetSamples' from it.
			/// </summary>
			public IAudioNode inputNode;
			
			#region IAudioNode implementation
			/// <summary>
			/// The next audio node in the graph (in this case the mixer) will require sample data from this
			/// node, so we implement the IAudioNode interface.
			/// </summary>
			/// <returns>
			/// The sample processed from the input IAudioNode
			/// </returns>
			public double GetSample ()
			{
				//get the sample from the previous node in the graph
				double sample = inputNode.GetSample();
				
				//process the sample 
				if(sample > 1 || sample < -1) //don't do any processing if we don't need to!
				{
					double scale = Math.Abs(1.0 / sample); //EG 1 / 1.5 = 0.6;
					sample *= scale; //EG 1.5 * 0.6 = 0.9;
				}
				
				//give the sample to the next node in the graph
				return sample;
			}
			#endregion
			
		}
	}
}
