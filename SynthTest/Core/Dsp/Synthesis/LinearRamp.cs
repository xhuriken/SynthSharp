using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Core.Dsp.Synthesis
{
    public class LinearRamp
    {
        private float _currentValue;
        private float _targetValue;
        private float _increment;
        private int _samplesRemaining;
        private readonly float _sampleRate;
        private readonly float _rampTimeSeconds;

        public LinearRamp(float sampleRate, float rampTimeSeconds = 0.05f)
        {
            _sampleRate = sampleRate;
            _rampTimeSeconds = rampTimeSeconds;
        }

        public float Value
        {
            get => _currentValue;
            set
            {
                if (_targetValue == value) return;

                _targetValue = value;
                // Combien d'echantillons sont necessaires pour atteindre la value cible
                _samplesRemaining = (int)(_sampleRate * _rampTimeSeconds);

                // Calcul de l'increment par d'echantillon pour atteindre la value cible en temps voulu
                if (_samplesRemaining > 0)
                {
                    _increment = (_targetValue - _currentValue) / _samplesRemaining;
                }
                else
                {
                    _currentValue = _targetValue;
                    _increment = 0;
                }
            }
        }

        // Appel a chaque sample dans le Generate() de ISignalSource
        public float Next()
        {
            if (_samplesRemaining > 0)
            {
                _currentValue += _increment;
                _samplesRemaining--;
            }
            else
            {
                _currentValue = _targetValue;
            }
            return _currentValue;
        }
    }
}