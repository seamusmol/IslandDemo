using System;
using mob;
using terrain;
using System.Windows.Media.Media3D;
using Game;
using Sky;

namespace main
{
    /*
     * TODO
     * System to generate random location
     * 
     * Mob Management->NavMesh, AI behaviour
     * Inventory System
     * Terrain destruction Manager
     * Jumping System
     * Menu System
     */

    
    public class GameState : AppState
    {
        Boolean HasActiveGame = true;
        ApplicationInputManager Input;
        MainWindow MainWindow;
        public Player Player { get; set; }
        RenderManager RenderManager;
        ChunkTracker ChunkTracker;
        MobManager MobManager;
        SkyDomeAppState SkyDome;

        public GameState()
        {
            
        }
        
        override
        public void initialize(MainWindow mainWindow)
        {
            VoxelModels.VoxelModels.LoadModels();
            MainWindow = mainWindow;
            Input = MainWindow.Input;
            ChunkTracker = new ChunkTracker(ApplicationSettings.SX, ApplicationSettings.SY);
            Player = new Player(new Point3D(ApplicationSettings.worldSize*ApplicationSettings.chunkSize/2, 20.5f, ApplicationSettings.worldSize * ApplicationSettings.chunkSize/2), ChunkTracker);
            RenderManager = mainWindow.RenderManager;
            MobManager = new MobManager(RenderManager,ChunkTracker, Player);
            SkyDome = new SkyDomeAppState();
           
            MainWindow.appStateManager.addAppState("Player", Player);
            MainWindow.appStateManager.addAppState("SkyDome", SkyDome);
            MainWindow.appStateManager.addAppState("ChunkTracker", ChunkTracker);
            MainWindow.appStateManager.addAppState("MobManager", MobManager);

        }
        
        override
        public void update(float tpf, long framestart)
        {
            if (Input.hasInput("P"))
            {
                if (HasActiveGame)
                {
                    //toggle to menu mode
                    Player.isEnabled = false;
                    ChunkTracker.isEnabled = false;
                }
                else
                {
                    //toggle to game mode
                    Player.isEnabled = true;
                    ChunkTracker.isEnabled = true;
                }
            }
        }
        
        override
        public void close()
        {
        }
    }
}
