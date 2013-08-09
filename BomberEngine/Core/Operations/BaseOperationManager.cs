using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Game;
using BomberEngine.Util;

namespace BomberEngine.Core.Operations
{
    class BaseOperationManager : IDestroyable
    {
        private TimerManager m_timerManager;
        private ObjectsPool<BaseOperation> m_operationsPool;

        public BaseOperationManager()
            : this(Application.TimerManager())
        {
        }

        public BaseOperationManager(TimerManager timerManager)
        {
            m_timerManager = timerManager;
            m_operationsPool = new ObjectsPool<BaseOperation>(timerManager);
        }

        public void AddOperation(BaseOperation op)
        {

        }

        public void Destroy()
        {   
        }
    }
}
