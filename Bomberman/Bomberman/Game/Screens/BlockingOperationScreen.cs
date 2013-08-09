using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Core.Visual;
using BomberEngine.Core.Operations;

namespace Bomberman.Game.Screens
{
    public class BlockingOperationScreen : BlockingScreen, IBaseOperationListener
    {
        private BaseOperation m_operation;

        public BlockingOperationScreen(String message, BaseOperation op)
            : base(message)
        {
            m_operation = op;
            m_operation.AddListener(this);
        }

        public override void Destroy()
        {
            m_operation.RemoveListener(this);
            base.Destroy();
        }

        protected override void OnCancel()
        {
            m_operation.Cancel();
            base.OnCancel();
        }

        public void OnOperationStarted(BaseOperation op)
        {
        }

        public void OnOperationFinished(BaseOperation op)
        {
            Finish();
        }
    }
}
