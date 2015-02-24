using System;
using System.Collections.Generic;
using System.Drawing;
using AForge.Neuro;
using AForge.Neuro.Learning;

namespace BoozeMaster
{
	class ColorMomentANNClassifier : IClassifier
	{
		public ColorMomentANNClassifier(int classCount)
		{
			this.classCount = classCount;
		}

		public void Train(IList<ClassifiedTrademark> trainingSet)
		{
			var inputs = new double[trainingSet.Count][];
			var outputs = new double[trainingSet.Count][];
			for(int i = 0; i < trainingSet.Count; i++)
			{
				inputs[i] = featureExtractor.ExtractFeatures(trainingSet[i].Image);
				outputs[i] = new double[classCount];
				outputs[i][trainingSet[i].TrademarkClass] = 1;
			}

			ann = new ActivationNetwork(new SigmoidFunction(), featureExtractor.Dimension, 12, classCount);//12
			ann.Randomize();
			var teacher = new BackPropagationLearning(ann) { LearningRate = 0.1 };

			const int iterations = 100000;
			const int reportFreq = iterations / 20;
			for (int i = 0; i < iterations; i++)
			{
				var e = teacher.RunEpoch(inputs, outputs) / trainingSet.Count;
				if(i % reportFreq == 0)
					Console.WriteLine("Training. " + GetType().Name + " error rate: " + e);
				if (Math.Abs(e) < 1e-4)
					break;
			}
		}

		public int Classify(Bitmap trademark)
		{
			var output = ann.Compute(featureExtractor.ExtractFeatures(trademark));
			return Array.FindIndex(output, x => x >= 0.8);
		}

		ActivationNetwork ann;
		readonly IFeatureExtractor featureExtractor = new HslColorMomentsExtractor();
		readonly int classCount;
	}
}