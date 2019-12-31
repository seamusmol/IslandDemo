
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Game;

namespace main
{
    public class AppStateManager
    {
        Dictionary<string, AppState>  appStateList = new Dictionary<string, AppState>();
        Dictionary<string, AppState> appStateQueueList = new Dictionary<string, AppState>();

        MainWindow MainWindow;

        public AppStateManager(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        public void processAppStates(float tpf, long frametime)
        {
            foreach (KeyValuePair<string, AppState> entry in appStateQueueList)
            {
                appStateList.Add(entry.Key, entry.Value);
            }
            appStateQueueList.Clear();
            
            foreach (KeyValuePair<string, AppState> entry in appStateList)
            {
                if (entry.Value.needsRemoval)
                {
                    entry.Value.close();
                    appStateList.Remove(entry.Key);
                }
            }
            
            foreach (KeyValuePair<string, AppState> entry in appStateList)
            {
                if (!entry.Value.hasInitialized)
                {
                    entry.Value.initialize(MainWindow);
                    entry.Value.hasInitialized = true;
                    entry.Value.isEnabled = true;
                }
            }
            foreach (KeyValuePair<string, AppState> entry in appStateList)
            {
                if (entry.Value.isEnabled)
                {
                    entry.Value.update(tpf, frametime);
                }
            }
        }

        public Dictionary<string,AppState> getAppStates()
        {
            return appStateList;
        }

        public AppState getAppState(string Name)
        {
            foreach (KeyValuePair<string, AppState> entry in appStateList)
            {
                if (entry.Key.Equals(Name))
                {
                    return entry.Value;
                }
            }
            return null;
        }
        public void removeAppStateByName(string Name)
        {
            foreach (KeyValuePair<string, AppState> entry in appStateList)
            {
                if (entry.Key.Equals(Name))
                {
                    //appStateList.Remove(entry.Key);
                    return;
                }
            }
        }

        public void setAppStateEnabled(string Name, bool Value)
        {
            foreach (KeyValuePair<string, AppState> entry in appStateList)
            {
                if (entry.Key.Equals(Name))
                {
                    entry.Value.isEnabled = Value;
                    return;
                }
            }
        }

        public void addAppState(string Name, AppState AppState)
        {
            if (appStateList.ContainsKey(Name) || appStateQueueList.ContainsKey(Name))
            {
                return;
            }
            appStateQueueList.Add(Name, AppState);
        }

    }



}