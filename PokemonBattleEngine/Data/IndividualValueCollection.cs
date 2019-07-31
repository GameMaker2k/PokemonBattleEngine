﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngine.Data
{
    // TODO: Listen to settings changes
    public sealed class PBEIndividualValueCollection : IEnumerable<PBEIndividualValueCollection.PBEIndividualValue>, INotifyPropertyChanged
    {
        public sealed class PBEIndividualValue : INotifyPropertyChanged
        {
            private void OnPropertyChanged(string property)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
            public event PropertyChangedEventHandler PropertyChanged;

            private readonly PBEIndividualValueCollection parent;

            public PBEStat Stat { get; }
            private byte value;
            public byte Value
            {
                get => value;
                set
                {
                    if (this.value != value)
                    {
                        parent.Set(Stat, value);
                    }
                }
            }

            internal PBEIndividualValue(PBEIndividualValueCollection parent, PBEStat stat)
            {
                this.parent = parent;
                Stat = stat;
            }

            internal void Update(byte value)
            {
                this.value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly PBESettings settings;
        private PBEIndividualValue[] ivs;

        private PBEType hiddenPowerType;
        public PBEType HiddenPowerType
        {
            get => hiddenPowerType;
            private set
            {
                if (hiddenPowerType != value)
                {
                    hiddenPowerType = value;
                    OnPropertyChanged(nameof(HiddenPowerType));
                }
            }
        }
        private byte hiddenPowerBasePower;
        public byte HiddenPowerBasePower
        {
            get => hiddenPowerBasePower;
            private set
            {
                if (hiddenPowerBasePower != value)
                {
                    hiddenPowerBasePower = value;
                    OnPropertyChanged(nameof(HiddenPowerBasePower));
                }
            }
        }

        public PBEIndividualValueCollection(PBESettings settings, bool randomize)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            this.settings = settings;
            CreateIVs();
            if (randomize)
            {
                Randomize();
            }
        }
        public PBEIndividualValueCollection(PBESettings settings, byte hp = 0, byte attack = 0, byte defense = 0, byte spAttack = 0, byte spDefense = 0, byte speed = 0)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            this.settings = settings;
            CreateIVs();
            Set(PBEStat.HP, hp);
            Set(PBEStat.Attack, attack);
            Set(PBEStat.Defense, defense);
            Set(PBEStat.SpAttack, spAttack);
            Set(PBEStat.SpDefense, spDefense);
            Set(PBEStat.Speed, speed);
        }
        public PBEIndividualValueCollection(PBESettings settings, PBEIndividualValueCollection other)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            this.settings = settings;
            CreateIVs();
            Set(PBEStat.HP, other[PBEStat.HP].Value);
            Set(PBEStat.Attack, other[PBEStat.Attack].Value);
            Set(PBEStat.Defense, other[PBEStat.Defense].Value);
            Set(PBEStat.SpAttack, other[PBEStat.SpAttack].Value);
            Set(PBEStat.SpDefense, other[PBEStat.SpDefense].Value);
            Set(PBEStat.Speed, other[PBEStat.Speed].Value);
        }
        private void CreateIVs()
        {
            ivs = new PBEIndividualValue[6]
            {
                new PBEIndividualValue(this, PBEStat.HP),
                new PBEIndividualValue(this, PBEStat.Attack),
                new PBEIndividualValue(this, PBEStat.Defense),
                new PBEIndividualValue(this, PBEStat.SpAttack),
                new PBEIndividualValue(this, PBEStat.SpDefense),
                new PBEIndividualValue(this, PBEStat.Speed)
            };
            UpdateHiddenPower();
        }
        private void UpdateHiddenPower()
        {
            HiddenPowerType = PBEPokemonData.GetHiddenPowerType(this[PBEStat.HP].Value, this[PBEStat.Attack].Value, this[PBEStat.Defense].Value, this[PBEStat.SpAttack].Value, this[PBEStat.SpDefense].Value, this[PBEStat.Speed].Value);
            HiddenPowerBasePower = PBEPokemonData.GetHiddenPowerBasePower(this[PBEStat.HP].Value, this[PBEStat.Attack].Value, this[PBEStat.Defense].Value, this[PBEStat.SpAttack].Value, this[PBEStat.SpDefense].Value, this[PBEStat.Speed].Value);
        }

        public PBEIndividualValue this[PBEStat stat]
        {
            get
            {
                int statIndex = (int)stat;
                if (statIndex >= 6)
                {
                    throw new ArgumentOutOfRangeException(nameof(stat));
                }
                else
                {
                    return ivs[statIndex];
                }
            }
        }

        public void Set(PBEStat stat, byte value)
        {
            int statIndex = (int)stat;
            if (statIndex >= 6)
            {
                throw new ArgumentOutOfRangeException(nameof(stat));
            }
            byte newVal = Math.Min(value, settings.MaxIVs);
            PBEIndividualValue iv = ivs[statIndex];
            if (iv.Value != newVal)
            {
                iv.Update(newVal);
                UpdateHiddenPower();
            }
        }

        public void Randomize()
        {
            for (int i = 0; i < 6; i++)
            {
                ivs[i].Value = (byte)PBEUtils.RandomInt(0, settings.MaxIVs);
            }
        }

        public IEnumerator<PBEIndividualValue> GetEnumerator()
        {
            for (int i = 0; i < 6; i++)
            {
                yield return ivs[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
