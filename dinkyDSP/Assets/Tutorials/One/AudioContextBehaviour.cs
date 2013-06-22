using UnityEngine;
using System.Collections.Generic;
using com.lonewolfwilliams.dinkyDSP;

/*
 * Tutorial one
 * 
 * This introduces the basic setup for working with Dinky and gives a simple example of how 
 * to output a sound.
 * 
 * Typically every application of Dinky will have an audioContextBehaviour, just like this one
 * this is where you 'wire up' your audio graph:
 * 
 * An audio graph is a 'tree' of audioNodes that are connected together 'branching' out from a 
 * root node. Nodes could be sound generators, effect units, mixers, or something more complex. 
 * 
 * First you create instances of the audio nodes and configure them how you want them, in this 
 * example I have created a sine wave generator and a mixer, the mixers volume is configured to 
 * be at half way (0.5)
 * 
 * Next you 'wire the graph up', I have connected my generator as an input of the mixer, and 
 * designated the mixer as the 'root node' of the graph by adding it as the root node of the driver.
 * 
 * Finally you bind the driver to the OnAudioFilterRead callback of the AudioContext MonoBehaviour
 * this means passing the sample data from the driver (that will 'run' the graph you have constructed
 * to generate the sample data) to the buffer provided by Unity Engine (that will output the audio)  
 * 
 * When you run this scene you should hear a middle C sine wave (the default frequency for the generator)
 * 
 * What to do Next ?
 * 
 * SineWaveGenerator has a frequency property, perhaps you should try adding a frequency property on this
 * MonoBehaviour and binding it to the frequency property of the sineWaveGenerator ? Or perhaps adding 
 * an OnGUI callback and implement a slider that controls the pitch of the sine wave ?
 *
 */

namespace TutorialOne
{
	public class AudioContextBehaviour : MonoBehaviour 
	{
		Driver m_driver;
		
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
			var sineWave = new SineWaveGenerator();
			var mixer = new MonoMixer();
			mixer.masterOutputLevel = 0.5;
			
			//connect our graph
			mixer.AddInput(sineWave);
			m_driver.rootNode = mixer;
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
