using System;
using Game;

namespace main
{
    public abstract class AppState
    {
        public Boolean isEnabled { get; set; } = false;
        public Boolean hasInitialized { get; set; } = false;
        public Boolean needsRemoval { get; } = false;
        
	    public AppState()
	    {

	    }

        public abstract void initialize(MainWindow MainWindow);

        public abstract void update(float tpf, long FrameTime);

        public abstract void close();
    }
}

