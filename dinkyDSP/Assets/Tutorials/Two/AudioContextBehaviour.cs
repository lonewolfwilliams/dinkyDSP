using UnityEngine;
using System.Collections.Generic;
using com.lonewolfwilliams.dinkyDSP;

/*
 * Tutorial two
 * 
 * This tutorial shows some of the properties of the audioNodes in Dinky and simple ways
 * you might bind them to your game, it also introduces the "common" definitions utility class
 * 
 * Following on from the previous tutorial we have a graph consisting of a Sinewave generator 
 * wired into a mixer that is defined as the root node of the graph on the driver. The OnAudioFilterRead
 * callback transfers the sample data generated from the graph by the driver into the sample buffer 
 * of the Unity Engine.
 * 
 * Generators (which can be found in DinkyDSP.Core.Generators) produce waveforms with an amplitude and frequency,
 * the amplitude is the height of the waveform (how loud you perceive it) and the frequency is how
 * quickly one period of the waveform repeats (the pitch you perceive it). It is important to note,
 * therefore that Generators have no concept of notes, only frequencies (the pitch of a note.)
 * 
 * Notice below how the frequency property of the SineWaveGenerator audio node is bound to a GUI slider, 
 * the range of the slider is between the note of C at octave two (defined as lowOctave) and the the note of C at 
 * octave four (defined as highOctave) the frequencies that map to these notes are stored in an 
 * array in the 'Common' utility class. To get different octaves we can multiply the frequency by the octave
 * we require, so "C3" becomes Common.noteToFrequency["C"] * 3;
 * 
 * The master volume of the mixer is bound to the volume property exposed to the inspector on this Monobehaviour
 * component. If you 'play' this scene and then look at this component in the inspector you will see that 
 * there is an output meter that has appeared - this is automagically added by Unity Engine when you define the
 * OnAudioFilterRead callback. If you change the volume property you will see how the output increases, if you go
 * beyond the value of one (Be careful with your speaker volume for your OS! and your ears!) you will hear that
 * sinewave becomes distorted, this is the phenomena of 'clipping' you should try to make sure that the
 * amplitude of the signal you generate in your graph stays in the range of 0 to 1 if you want to avoid clipping.
 * 
 * What to do next ?
 * 
 * If you look at the Common class it contains arrays of notes that make up various scales, 
 * try and 'lock' the slider so that it can only produce frequencies that are note values. 
 *
 * Try and limit the volume property so that the user can't accidentally create clipping.
 * 
 */

namespace TutorialTwo
{
	public class AudioContextBehaviour : MonoBehaviour 
	{
		public double volume = 0.5;
		
		Driver m_driver;
		SineWaveGenerator m_sineWave;
		MonoMixer m_mixer;
		
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
			m_mixer = new MonoMixer();
			m_mixer.masterOutputLevel = 1.0;
			
			//connect our graph
			m_mixer.AddInput(m_sineWave);
			m_driver.rootNode = m_mixer;
		}
		
		void Update()
		{
			//expose the volume as a component property in the inspector
			m_mixer.masterOutputLevel = volume;
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
	}
}
