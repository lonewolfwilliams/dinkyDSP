using System;

/*
 * 	Gareth Williams
 * 
 *	this is an example of how you may want to make nested graph segments for more complex IAudioNodes
 * 	this factory creates a preconfigured FMSynth with a step sequencer
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public static class SynthFactory
	{
		public enum Voice
		{
			noise,
			pulse,
			saw,
			sine
		}
		
		static Random m_rnd = new Random();
			
		public static BaseSynth Create(Voice carrier, Voice modulator)
		{
			BaseSynth synth = new BaseSynth();
			
			FMSynth fm 	 = new FMSynth();	
			fm.Modulator = getGeneratorForVoice(carrier);
			fm.Carrier 	 = getGeneratorForVoice(modulator);
			
			fm.harmonicRatio = m_rnd.Next(1,4); //whole numbers are harmonic
			fm.modulationIndex = (float)m_rnd.NextDouble();
			
			synth.inputNode = fm as IAudioNode;
				
			return synth;
		}
		
		#region helper functions
		
		static IAudioNode getGeneratorForVoice (Voice carrier)
		{
			IAudioNode generator;
			switch (carrier) 
			{
			case Voice.noise:
				generator = new NoiseWaveGenerator();
				break;
			case Voice.pulse:
				generator = new PulseWaveGenerator();
				break;
			case Voice.saw:
				generator = new SawtoothWaveGenerator();
				break;
			default:
			case Voice.sine:
				generator = new SineWaveGenerator();
				break;
			}
			
			return generator;
		}
		
		#endregion helper functions
	}
	
	#region helper classes
	/*
	*	Gareth Williams
	*
	*	An FM synth with variable harmonicity and brightness 
	*	(ensures randoms will generate something audible and not speaker blowing)
	*
	*/
		
	internal class FMSynth : IAudioNode, IHasPitch
	{
		public float modulationDepth = 22000;
		public float harmonicRatio = 1.0f;
		public float modulationIndex = 1.0f;
		
		#region accessors
		float m_carrierFrequency = 32.70f * 3;
		public float Frequency
		{
			get
			{
				return m_carrierFrequency;	
			}
			set
			{
				m_carrierFrequency = value;
			}
		}
		
		IAudioNode m_modulator;
		public IAudioNode Modulator
		{
			get
			{
				return m_modulator;
			}
			set
			{
				m_modulator = value;
			}
		}
		
		IAudioNode m_carrier;
		public IAudioNode Carrier
		{
			get
			{
				return m_carrier;
			}
			set
			{
				m_carrier = value;
				
				if(m_carrier is IHasPitch)
				{
					(m_carrier as IHasPitch).Frequency = m_carrierFrequency;	
				}
			}
		}
		
		#endregion
		
		//http://www.cycling74.com/docs/max5/tutorials/msp-tut/mspchapter11.html
		#region IAudioNode implementation
		public double GetSample ()
		{
			if( m_modulator == null ||
				m_carrier == null)
			{
				return 0;
			}
			
			float harmonicity = m_carrierFrequency * harmonicRatio;
			float brightness = harmonicity * modulationIndex;
			
			if(Modulator is IHasPitch)
			{
				(Modulator as IHasPitch).Frequency = harmonicity;	
			}
			
			double modulatorAmplitude = Modulator.GetSample() * brightness;
			
			if(Carrier is IHasPitch)
			{
				(Carrier as IHasPitch).Frequency = m_carrierFrequency + (float)modulatorAmplitude;	
			}
			
			var sample = Carrier.GetSample();
			
			return sample;
		}
		#endregion
	}
	#endregion helper classes
}

