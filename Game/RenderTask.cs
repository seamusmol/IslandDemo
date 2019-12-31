using System;
using System.Windows.Media.Media3D;

namespace main
{
    public class RenderTask
    {
	    public RenderTask()
	    {
            
	    }
       
        public class RotateTask : RenderTask
        {
            public RotateTask()
            {

            }
        }
        
        public class UploadTask : RenderTask
        {
            public GeometryModel3D Model { get; set; }
            public UploadTask(GeometryModel3D UploadModel)
            {
                Model = UploadModel;
            }
        }

    }
}

