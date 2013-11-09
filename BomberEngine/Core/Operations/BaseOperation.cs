using System;

namespace BomberEngine
{
    public class BaseOperation : ObjectsPoolEntry
    {
        private enum State
        {
            Created,
            Started,
            Finished,
            Cancelled,
            Failed,
        }

        private State m_state;
        private String m_errorMessage;

        public BaseOperation()
        {
            m_state = State.Created;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        public void Start(bool immediately = false)
        {
            Assert.IsTrue(m_state == State.Created);
            m_state = State.Started;

            OnStart();
        }

        public void Finish()
        {
            Assert.IsTrue(m_state == State.Started);
            m_state = State.Finished;

            OnFinish();
        }

        public void Cancel()
        {
            if (m_state == State.Started)
            {   
                m_state = State.Cancelled;
                OnFinish();
            }
        }

        public void Fail(String message)
        {
            Assert.IsTrue(m_state == State.Started);
            m_state = State.Failed;
            m_errorMessage = message;

            OnFinish();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Inheritance

        protected virtual void DoWork()
        {
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnFinish()
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Properties

        public bool IsStarted
        {
            get { return m_state == State.Started; }
        }

        public bool IsFinised
        {
            get { return m_state == State.Finished; }
        }

        public bool IsCancelled
        {
            get { return m_state == State.Cancelled; }
        }

        public bool IsFailed
        {
            get { return m_state == State.Failed; }
        }

        public String ErrorMessage
        {
            get { return m_errorMessage; }
        }

        #endregion
    }
}
