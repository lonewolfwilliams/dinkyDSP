using System;

/*
 * Gareth Williams
 * 
 * mono
 * 
 * The oh-so important filter implementation - implementation adapted from NAudio
 * //http://naudio.codeplex.com/SourceControl/changeset/view/2339a04e76cc#NAudio/Dsp/BiQuadFilter.cs
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class LowPassFilter : IAudioNode, IHasInput
	{
		public double cutoffFrequency = 800;
		public double resonance = 1.5;
		
		#region IHasInput
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
		
		// coefficients
        double a0;
        double a1;
        double a2;
        double b0;
        double b1;
        double b2;
		
		double[] x = new double[3];
        double[] y = new double[3];

		#region IAudioNode implementation
		public event SampleEventHandler SampleGenerated;
		public double GetSample ()
		{
			if(m_inputNode == null)
			{
				return 0;
			}
			
			double sampleRate = Driver.sampleRate;
			double w0 = 2 * Math.PI * cutoffFrequency / sampleRate;
            double cosw0 = Math.Cos(w0);
            double alpha = Math.Sin(w0) / (2 * resonance);

           	b0 =  (1 - cosw0) / 2;
            b1 =   1 - cosw0;
            b2 =  (1 - cosw0) / 2;
            a0 =   1 + alpha;
            a1 =  -2 * cosw0;
            a2 =   1 - alpha;
			
			double sampleIn = m_inputNode.GetSample();
			
			x[0] = x[1];
			x[1] = x[2];
			x[2] = sampleIn;
			
			y[0] = y[1];
			y[1] = y[2];
			y[2] = (
            	(b0/a0)*x[2] + (b1/a0)*x[1] + (b2/a0)*x[0]
                - (a1/a0)*y[1] - (a2/a0)*y[0]);
			
			double sampleOut = y[2];
			if(SampleGenerated != null)
			{
				SampleGenerated(sampleOut);	
			}
			
			return sampleOut;
		}
		#endregion
		
	}
}

