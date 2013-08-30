using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Demo
{
    public interface IDemoManager
    {
        void Save(String path);
        void Load(String path);
    }

    public class DemoRecordManager : IDemoManager
    {
        private DemoRecorder m_recorder;

        public DemoRecordManager()
        {
            m_recorder = new DemoRecorder();
        }

        public void Save(String path)
        {
            m_recorder.Save(path);
        }

        public void Load(String path)
        {
            throw new NotImplementedException();
        }
    }

    public class DemoPlaybackManager : IDemoManager
    {
        private DemoPlayer m_player;

        public DemoPlaybackManager()
        {
            m_player = new DemoPlayer();
        }

        public void Save(String path)
        {
            throw new NotImplementedException();
        }

        public void Load(String path)
        {
            throw new NotImplementedException();
        }
    }
}
