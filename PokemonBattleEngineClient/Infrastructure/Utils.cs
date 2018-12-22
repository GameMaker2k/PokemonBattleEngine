﻿using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kermalis.PokemonBattleEngineClient.Infrastructure
{
    static class Utils
    {
        public static bool DoesResourceExist(string resource)
        {
            string[] resources = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            return resources.Contains(resource);
        }
        public static Bitmap UriToBitmap(Uri uri)
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            return new Bitmap(assets.Open(uri));
        }

        static readonly Dictionary<string, Bitmap> LoadedBitmaps = new Dictionary<string, Bitmap>();
        static string GetCharKey(char c)
        {
            string key = ((int)c).ToString("X");
            string questionMark = ((int)'?').ToString("X");
            return DoesResourceExist($"Kermalis.PokemonBattleEngineClient.Assets.Fonts.{key}.png") ? key : questionMark;
        }
        public static Bitmap RenderString(string str)
        {
            // Return null for bad strings
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            // Load char bitmaps
            foreach (char c in str)
            {
                if (c == ' ' || c == '\r' || c == '\n')
                {
                    continue;
                }
                else
                {
                    string key = GetCharKey(c);
                    if (!LoadedBitmaps.ContainsKey(key))
                    {
                        LoadedBitmaps.Add(key, UriToBitmap(new Uri($"resm:Kermalis.PokemonBattleEngineClient.Assets.Fonts.{key}.png?assembly=PokemonBattleEngineClient")));
                    }
                }
            }

            // Measure how large the string will end up
            const int charHeight = 15;
            int lineWidth = 0, stringHeight = charHeight;
            var eachLineWidth = new List<int>();
            foreach (char c in str)
            {
                if (c == ' ')
                {
                    lineWidth += 4;
                }
                else if (c == '\r')
                {
                    continue;
                }
                else if (c == '\n')
                {
                    stringHeight += charHeight + 1;
                    eachLineWidth.Add(lineWidth);
                    lineWidth = 0;
                }
                else
                {
                    lineWidth += LoadedBitmaps[GetCharKey(c)].PixelSize.Width;
                }
            }
            eachLineWidth.Add(lineWidth);

            // Draw the string
            var rtb = new RenderTargetBitmap(new PixelSize(eachLineWidth.Max(), stringHeight));
            using (var ctx = rtb.CreateDrawingContext(null))
            {
                double x = 0, y = 0;
                foreach (char c in str)
                {
                    if (c == ' ')
                    {
                        x += 4;
                    }
                    else if (c == '\r')
                    {
                        continue;
                    }
                    else if (c == '\n')
                    {
                        y += charHeight + 1;
                        x = 0;
                    }
                    else
                    {
                        Bitmap bmp = LoadedBitmaps[GetCharKey(c)];
                        int charWidth = bmp.PixelSize.Width;
                        ctx.DrawImage(bmp.PlatformImpl, 1.0, new Rect(0, 0, charWidth, charHeight), new Rect(x, y, charWidth, charHeight));
                        x += charWidth;
                    }
                }
            }
            return rtb;
        }
    }
}
