using SynthTest.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthTest.Core.Dsp.Generators
{
    /// <summary>
    /// The different states of the ADSR envelope.
    /// </summary>
    public enum AdsrState { Idle, Attack, Decay, Sustain, Release }

    /// <summary>
    /// Generate a control signal (CV) between 0.0 and 1.0 to shape the volume or the filter over time.
    /// It does not process audio, it creates a modulation curve.
    /// </summary>
    public class AdsrNode : IAudioNode
    {
        // The actual state of this adsr node instance
        private AdsrState _state = AdsrState.Idle;
        // the current value that adsr will return in function of time
        private float _currentValue = 0.0f;

        #region ADSR PARAMETERS

        // TODO: LINK WITH LINEAR RAMP

        /// <summary>
        /// Time in seconds to reach the maximum level (1.0).
        /// </summary>
        public float AttackTime { get; set; } = 0.1f;

        /// <summary>
        /// Time in seconds to drop from maximum level to the Sustain level.
        /// </summary>
        public float DecayTime { get; set; } = 0.1f;

        /// <summary>
        /// The level (between 0.0 and 1.0) where the sound stays as long as the key is pressed.
        /// </summary>
        public float SustainLevel { get; set; } = 0.8f;

        /// <summary>
        /// Time in seconds to drop from Sustain level to zero when the key is released.
        /// </summary>
        public float ReleaseTime { get; set; } = 0.5f;

        #endregion

        // --- Gate Logic ---

        private bool _isGateOpen = false;

        /// <summary>
        /// Gets or sets the gate status. When true, the envelope starts (Attack). 
        /// When false, the envelope stops and falls (Release).
        /// FOR NOW ITS TOGGLE BUTTON, later we will read MIDI
        /// </summary>
        public bool IsGateOpen
        {
            get => _isGateOpen;
            set
            {
                if (_isGateOpen == value) return;
                _isGateOpen = value;

                if (_isGateOpen)
                {
                    // Key pressed ! We start the attack from where we are.
                    _state = AdsrState.Attack;
                }
                else
                {
                    // Key released ! We drop to zero.
                    _state = AdsrState.Release;
                }
            }
        }

        public void ProcessBlock(float[] buffer, int offset, int count, AudioContext context)
        {
            // We calculate increments outside the loop for CPU performance
            // Formula: distance / (time * sampleRate)
            float attackRate = AttackTime > 0.001f ? 1.0f / (AttackTime * context.SampleRate) : 1.0f;
            float decayRate = DecayTime > 0.001f ? (1.0f - SustainLevel) / (DecayTime * context.SampleRate) : 1.0f;
            float releaseRate = ReleaseTime > 0.001f ? SustainLevel / (ReleaseTime * context.SampleRate) : 1.0f;

            for (int i = 0; i < count; i++)
            {
                switch (_state)
                {
                    case AdsrState.Idle:
                        _currentValue = 0.0f;
                        break;

                    case AdsrState.Attack:
                        _currentValue += attackRate;
                        if (_currentValue >= 1.0f)
                        {
                            _currentValue = 1.0f;
                            _state = AdsrState.Decay; // Attack is done, move to Decay
                        }
                        break;

                    case AdsrState.Decay:
                        _currentValue -= decayRate;
                        if (_currentValue <= SustainLevel)
                        {
                            _currentValue = SustainLevel;
                            _state = AdsrState.Sustain; // Decay is done, hold the Sustain
                        }
                        break;

                    case AdsrState.Sustain:
                        _currentValue = SustainLevel; // We stay here until IsGateOpen becomes false
                        break;

                    case AdsrState.Release:
                        _currentValue -= releaseRate;
                        if (_currentValue <= 0.0f)
                        {
                            _currentValue = 0.0f;
                            _state = AdsrState.Idle; // Envelope is finished, waiting for next note
                        }
                        break;
                }

                // Write the generated CV value into the buffer
                buffer[offset + i] = _currentValue;
            }
        }
    }
}