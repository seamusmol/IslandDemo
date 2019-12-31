using System;

namespace main
{
    public static class ApplicationSettings
    {
        public static float FPS { get; } = 60;
        public static int chunkSize { get; } = 16;
        public static int renderDistance { get; } = 7;
        public static int blockSize { get;} = 1;
        public static int worldSize { get; } = 64;
        public static int worldHeight { get; } = 17;

        public static int SX { get; } = 321;
        public static int SY { get; } = 321;

        public static double mouse_sens { get; } = 2.0d;
        public static int seaLevel { get; } = 2;

        public static int SkyDomeDistance { get; } = 250;
    }
}


