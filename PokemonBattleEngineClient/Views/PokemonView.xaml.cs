﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kermalis.PokemonBattleEngine.Data;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using System;
using System.ComponentModel;

namespace Kermalis.PokemonBattleEngineClient.Views
{
    class PokemonView : UserControl, INotifyPropertyChanged
    {
        void OnPropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        public new event PropertyChangedEventHandler PropertyChanged;

        PBEPokemon pokemon;
        public PBEPokemon Pokemon
        {
            get => pokemon;
            set
            {
                pokemon = value;
                OnPropertyChanged(nameof(Pokemon));
            }
        }
        double scale;
        double Scale
        {
            get => scale;
            set
            {
                scale = value;
                OnPropertyChanged(nameof(Scale));
            }
        }
        double spriteOpacity;
        double SpriteOpacity
        {
            get => spriteOpacity;
            set
            {
                spriteOpacity = value;
                OnPropertyChanged(nameof(SpriteOpacity));
            }
        }
        bool visible;
        bool Visible
        {
            get => visible;
            set
            {
                visible = value;
                OnPropertyChanged(nameof(Visible));
            }
        }
        Point location;
        public Point Location
        {
            get => location;
            set
            {
                location = value;
                OnPropertyChanged(nameof(Location));
            }
        }
        Uri uri;
        Uri Source
        {
            get => uri;
            set
            {
                uri = value;
                OnPropertyChanged(nameof(Source));
            }
        }

        public PokemonView()
        {
            AvaloniaXamlLoader.Load(this);
            DataContext = this;
        }

        public void Update()
        {
            if (pokemon == null || pokemon.FieldPosition == PBEFieldPosition.None)
            {
                Visible = false;
            }
            else
            {
                Scale = pokemon.LocalTeam ? 2 : 1;

                // Fly/Bounce/SkyDrop / Dig / Dive / ShadowForce
                SpriteOpacity = !pokemon.Status2.HasFlag(PBEStatus2.Substitute) && (pokemon.Status2.HasFlag(PBEStatus2.Airborne) || pokemon.Status2.HasFlag(PBEStatus2.Underground) || pokemon.Status2.HasFlag(PBEStatus2.Underwater)) ? 0.4 : 1.0;

                string orientation = pokemon.LocalTeam ? "-B" : "-F";
                if (pokemon.Status2.HasFlag(PBEStatus2.Substitute))
                {
                    Source = new Uri($"resm:Kermalis.PokemonBattleEngineClient.Assets.Pokemon_Sprites.Substitute{orientation}.gif?assembly=PokemonBattleEngineClient");
                }
                else
                {
                    // Loading the correct sprite requires checking first
                    uint species = (uint)pokemon.Species & 0xFFFF;
                    uint forme = (uint)pokemon.Species >> 0x10;
                    string shiny = pokemon.Shiny ? "-S" : "";
                    string sss = string.Format("{0}{1}{2}{3}", species, forme > 0 ? $"-{forme}" : "", orientation, shiny);
                    // Following will be false if the species sprites are sss-M.gif and sss-F.gif
                    bool spriteIsGenderNeutral = Utils.DoesResourceExist($"Kermalis.PokemonBattleEngineClient.Assets.Pokemon_Sprites.{sss}.gif");
                    // sss.gif if the sprite is gender neutral, else sss-F.gif if the Pokémon is female, otherwise sss-M.gif
                    string gender = spriteIsGenderNeutral ? "" : pokemon.Shell.Gender == PBEGender.Female ? "-F" : "-M";
                    // Set the result
                    Source = new Uri($"resm:Kermalis.PokemonBattleEngineClient.Assets.Pokemon_Sprites.{sss}{gender}.gif?assembly=PokemonBattleEngineClient");
                }
                Visible = true;
            }
        }
    }
}
