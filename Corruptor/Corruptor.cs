using System;
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

        public void Corrupt(string file, string outputFile)
        {
            //announce start
            Console.WriteLine("Corrupting " + file);
            //corrupt bitmap image by getting the data without headers and randomising some bits
            var bitmap = new Bitmap(file);
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
                //shuffle the bytes in jumps of config.shuffleSize
                for (var i = 0; i < bytes.Length; i += config.shuffleSize)
                {
                    var j = random.Next(0, bytes.Length);
                    var temp = bytes[i];
                    bytes[i] = bytes[j];
                    bytes[j] = temp;
                }
            }

            if (config.chunkShuffle)
            {
                //shuffle the bytes in combined of config.chunkShuffleSize. This is done by swapping 2 arrays of bytes
                byte[] originalChunk = new byte[config.chunkShuffleSize];
                byte[] shuffledChunk = new byte[config.chunkShuffleSize];
                for (var i = 0; i < bytes.Length - config.chunkShuffleSize; i += config.chunkShuffleSize)
                {
                    //copy the original chunk to the originalChunk array
                    Array.Copy(bytes, i, originalChunk, 0, config.chunkShuffleSize);
                    //copy the shuffled chunk to the shuffledChunk array
                    Array.Copy(bytes, i + config.chunkShuffleSize, shuffledChunk, 0, config.chunkShuffleSize);
                    //swap the original and shuffled chunks
                    Array.Copy(shuffledChunk, 0, bytes, i, config.chunkShuffleSize);
                    Array.Copy(originalChunk, 0, bytes, i + config.chunkShuffleSize, config.chunkShuffleSize);
                }
            }
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
            bitmap.UnlockBits(data);
            //announce save
            Console.WriteLine("Saving " + outputFile);
            Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
            bitmap.Save(outputFile, bitmap.RawFormat);
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