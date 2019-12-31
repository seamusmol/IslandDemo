using System;
using System.Collections.Generic;
using Game;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows;
using System.Runtime.InteropServices;
using static main.ApplicationInputManager.NativeMethods;

namespace main
{
    public class ApplicationInputManager
    {
        public List<String> activeKeys { get; set; } = new List<String>();
        
        public Vector MouseMovement { get; set; } = new Vector();
       
        MainWindow MainWindow;
        
        public ApplicationInputManager(MainWindow mainWindow)
	    {
            MainWindow = mainWindow;
            Mouse.OverrideCursor = Cursors.None;
        }

        public partial class NativeMethods
        {
            [DllImportAttribute("user32.dll", EntryPoint = "SetCursorPos")]
            [return: MarshalAsAttribute(UnmanagedType.Bool)]
            public static extern bool SetCursorPos(int X, int Y);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetCursorPos(out POINT lpPoint);

            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                public int X;
                public int Y;

                public POINT(int x, int y)
                {
                    this.X = x;
                    this.Y = y;
                }
            }
        }
        

        public void update()
        {
            //clear keys
            activeKeys = new List<String>();

            if (MainWindow.HasFocus && MainWindow.IsActive)
            {
                if ( Keyboard.IsKeyDown(Key.W))
                {
                    activeKeys.Add("W");
                }
                if (Keyboard.IsKeyDown(Key.A))
                {
                    activeKeys.Add("A");
                }
                if (Keyboard.IsKeyDown(Key.S))
                {
                    activeKeys.Add("S");
                }
                if (Keyboard.IsKeyDown(Key.D))
                {
                    activeKeys.Add("D");
                }
                if (Keyboard.IsKeyDown(Key.P))
                {
                    activeKeys.Add("P");
                }
                if (Keyboard.IsKeyDown(Key.F5))
                {
                    activeKeys.Add("F5");
                }
                if (Keyboard.IsKeyDown(Key.F))
                {
                    activeKeys.Add("F");
                }
                
                if (Keyboard.IsKeyDown(Key.Escape))
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    MainWindow.HasFocus = false;
                }
                if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.Tab))
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    MainWindow.HasFocus = false;
                }

                System.Drawing.Rectangle resolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                int MX = (int)(MainWindow.Left + MainWindow.ActualWidth / 2);
                int MY = (int)(MainWindow.Top + MainWindow.ActualHeight / 2);

                POINT MousePosition = new POINT();
                if (GetCursorPos(out MousePosition))
                {
                    //MouseMovement = new Vector((MousePosition.X - MainWindow.ActualWidth) /MainWindow.ActualWidth, ( MousePosition.Y-MainWindow.ActualHeight) / MainWindow.ActualHeight);

                    MouseMovement = new Vector(MousePosition.X - MX, MousePosition.Y - MY);
                }
                else
                {
                    MouseMovement = new Vector();
                }

                if (System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    activeKeys.Add("M1");
                }
                if (System.Windows.Input.Mouse.RightButton == MouseButtonState.Pressed)
                {
                    activeKeys.Add("M2");
                }

                //Debug.WriteLine(MouseMovement);
                
                NativeMethods.SetCursorPos(MX,MY);
                //NativeMethods.SetCursorPos(800, 450);
            }

            
            
        }
        
        public Boolean hasInput(String KeyName)
        {
            return activeKeys.Contains(KeyName);
        }
        
    }
}

