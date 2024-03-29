﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace Corruptor
{
    public class Corruptor
    {
        public Config config;
        public Corruptor(string config)
        {
            this.config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(config));
        }
        
        public Corruptor(Config config)
        {
            this.config = config;
        }

        public void Corrupt(string file, string outputFile)
        {
            //announce start
            Console.WriteLine("Corrupting " + file);
            //corrupt bitmap image by getting the data without headers and randomising some bits
            Bitmap bitmap = CorruptImage(new Bitmap(file));
            //announce save
            Console.WriteLine("Saving " + outputFile);
            Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
            bitmap.Save(outputFile, bitmap.RawFormat);
        }

        public Bitmap CorruptImage(Bitmap original)
        {
            //announce start
            Console.WriteLine("Corrupting from Gui");
            Bitmap bitmap = (Bitmap) original.Clone();
            //corrupt bitmap image by getting the data without headers and randomising some bits
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            var bytes = new byte[data.Stride * data.Height];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            var random = new Random();
            if (config.perPixel)
            {
                for (var i = 0; i < bytes.Length; i++)
                {
                    //corrupt each pixel by shifting it by a random amount
                    bytes[i] = (byte)(bytes[i] << random.Next(0, 8));
                }
            }

            if (config.shuffle)
            {
                int shuffleSize = config.shuffleSize;
                if (shuffleSize == 0)
                {
                    int randomShuffleSize = random.Next(1, 16);
                    shuffleSize = randomShuffleSize;
                }
                Console.WriteLine("Shuffle size: " + shuffleSize);
                //shuffle the bytes in jumps of config.shuffleSize
                for (var i = 0; i < bytes.Length; i += Math.Min(1, random.Next(shuffleSize - 5, shuffleSize + 5)))
                {
                    if (i > bytes.Length - (shuffleSize + 5))
                    {
                        break;
                    }
                    var j = random.Next(0, bytes.Length);
                    (bytes[i], bytes[j]) = (bytes[j], bytes[i]);
                }
            }
            
            if (config.artifactAdd)
            {
                int artifactSize = config.artifactSize;
                //replace an array of bytes from the original image with a random array of bytes, multiple times, using Array.Copy()
                for (int v = 0; v < random.Next(config.maxArtifacts); v++)
                {
                    if (config.artifactSize == 0)
                    {
                        artifactSize = 4 * random.Next(1, bytes.Length/64);
                    }
                    int start = random.Next(bytes.Length - artifactSize);
                    byte[] artifact = new byte[artifactSize];
                    for (int i = 0; i < artifactSize; i++)
                    {
                        artifact[i] = (byte)random.Next(0, 256);
                    }
                    Array.Copy(artifact, 0, bytes, start, artifactSize);
                }
            }

            if (config.chunkShuffle)
            {
                int shuffleSize = config.chunkShuffleSize;
                if (shuffleSize == 0)
                {
                    int randomChunkSize = random.Next(1, bytes.Length);
                    shuffleSize = randomChunkSize;
                }

                Console.WriteLine("chunk size: " + shuffleSize);
                //shuffle the bytes in combined of config.chunkShuffleSize. This is done by swapping 2 arrays of bytes
                byte[] originalChunk = new byte[shuffleSize];
                byte[] shuffledChunk = new byte[shuffleSize];
                for (var i = 0; i < bytes.Length - shuffleSize * 2; i += shuffleSize)
                {
                    //copy the original chunk to the originalChunk array
                    Array.Copy(bytes, i, originalChunk, 0, shuffleSize);
                    //copy the shuffled chunk to the shuffledChunk array
                    Array.Copy(bytes, i + shuffleSize, shuffledChunk, 0, shuffleSize);
                    //swap the original and shuffled chunks
                    Array.Copy(shuffledChunk, 0, bytes, i, shuffleSize);
                    Array.Copy(originalChunk, 0, bytes, i + shuffleSize, shuffleSize);
                }
            }

            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            bitmap.UnlockBits(data);
            return bitmap;
        }
        
        public void CorruptDirectory(string directory, string outputFile)
        {
            foreach (var subDir in Directory.GetDirectories(directory))
            {
                CorruptDirectory(subDir, outputFile + "\\" + subDir.Substring(subDir.LastIndexOf('\\') + 1));
            }
            foreach (var file in Directory.GetFiles(directory))
            {
                Corrupt(file, outputFile + "\\" + file.Substring(file.LastIndexOf('\\') + 1));
            }
        }
    }
}