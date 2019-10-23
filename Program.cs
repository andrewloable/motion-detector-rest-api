using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AForge.Vision.Motion;
using CatchItREST.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace CatchItREST
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // initiate camera detection
            state.IsMotionDetected = false;
            state.IsCameraRunning = true;

            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (se, ev) =>
            {
                var frame = new Mat();
                var capture = new VideoCapture(0);
                capture.Open(0);
                Console.WriteLine("Connecting to first webcam...");
                var motionDetector = GetDefaultMotionDetector();
                Console.WriteLine("Initialize motion detector...");
                int counter = 0;
                if (capture.IsOpened())
                {
                    while (state.IsCameraRunning)
                    {
                        if (counter > 30)
                        {
                            counter = 0;
                            motionDetector.Reset();
                            Console.WriteLine("Motion Detector Reset...");
                        }
                        counter++;
                        capture.Read(frame);
                        var image = BitmapConverter.ToBitmap(frame);
                        if (image == null)
                            break;
                        var motionLevel = motionDetector.ProcessFrame(image);
                        
                        if (motionLevel > 0.50)
                            state.IsMotionDetected = true;
                        else
                            state.IsMotionDetected = false;
                        Console.WriteLine($"Motion Level: {motionLevel}");
                        Thread.Sleep(1000);
                    }
                }
            };
            bw.RunWorkerAsync();            

            CreateHostBuilder(args).Build().Run();
        }

        public static MotionDetector GetDefaultMotionDetector()
        {
            var detector = new SimpleBackgroundModelingDetector()
            {
                DifferenceThreshold = 10,
                FramesPerBackgroundUpdate = 10,
                KeepObjectsEdges = true,
                MillisecondsPerBackgroundUpdate = 0,
                SuppressNoise = true
            };
            var processor = new BlobCountingObjectsProcessing()
            {
                HighlightColor = System.Drawing.Color.Red,
                HighlightMotionRegions = true,
                MinObjectsHeight = 10,
                MinObjectsWidth = 10
            };
            var motionDetector = new MotionDetector(detector, processor);
            return motionDetector;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
