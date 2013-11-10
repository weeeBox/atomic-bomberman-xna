using System;
using BomberEngine;

namespace Bomberman.Gameplay.Screens
{
    public class BlockingOperationScreen : BlockingScreen
    {
        private BaseOperation m_operation;

        public BlockingOperationScreen(String message, BaseOperation op)
            : base(message)
        {
            m_operation = op;
            
        }

        public override void Destroy()
        {
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
