using SynthTest.Core.Abstractions;
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
        // Smoothing time
        private readonly float _rampTimeSeconds;

        public LinearRamp(float rampTimeSeconds = 0.05f)
        {
            _rampTimeSeconds = rampTimeSeconds;
        }

        public float Value
        {
            get => _currentValue; // Le DSP lit la valeur courante
            set
            {
                if (_targetValue == value) return;

                _targetValue = value;

                // SEULE DIFFÉRENCE : On prend le SampleRate du Config global
                _samplesRemaining = (int)(AudioConfig.SampleRate * _rampTimeSeconds);

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