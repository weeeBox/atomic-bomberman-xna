using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;
using BomberEngine.Game;

namespace BomberEngine.Core.Operations
{
    public interface IBaseOperationListener
    {
        void OnOperationStarted(BaseOperation op);
        void OnOperationFinished(BaseOperation op);
    }

    public abstract class BaseOperation
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

        private ITimerManager m_timerManager;
        private List<IBaseOperationListener> m_listeners;

        public BaseOperation()
            : this(Application.TimerManager())
        {
        }

        public BaseOperation(ITimerManager timerManager)
        {
            m_listeners = new List<IBaseOperationListener>();
            m_state = State.Created;
            m_timerManager = timerManager;
        }

        //////////////////////////////////////////////////////////////////////////////

        #region Lifecycle

        public void Start(bool immediately = false)
        {
            Debug.Assert(m_state == State.Started);
            m_state = State.Started;

            OnStart();
            NotifyStarted();

            if (!immediately)
            {
                m_timerManager.ScheduleOnce(StartTimerCallback);
            }
        }

        public void Finish()
        {
            Debug.Assert(m_state == State.Started);
            m_state = State.Finished;

            OnFinish();
            NotifyFinished();
        }

        public void Cancel()
        {
            if (m_state == State.Started)
            {
                m_timerManager.Cancel(StartTimerCallback);

                m_state = State.Cancelled;
                OnCancel();
                NotifyFinished();
            }
        }

        public void Fail(String message)
        {
            Debug.Assert(m_state == State.Started);
            m_state = State.Failed;
            m_errorMessage = message;

            OnFail();
            NotifyFinished();
        }

        private void StartTimerCallback(Timer timer)
        {
            DoWork();
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Inheritance

        protected abstract void DoWork();

        protected virtual void OnStart()
        {
        }

        protected virtual void OnFinish()
        {
        }

        protected virtual void OnCancel()
        {
        }

        protected virtual void OnFail()
        {
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////

        #region Delegates

        public void AddListener(IBaseOperationListener listener)
        {
            Debug.Assert(!m_listeners.Contains(listener));
            m_listeners.Add(listener);
        }

        public void RemoveListener(IBaseOperationListener listener)
        {
            Debug.Assert(m_listeners.Contains(listener));
            m_listeners.Remove(listener);
        }

        private void NotifyStarted()
        {
            for (int i = 0; i < m_listeners.Count; ++i)
            {
                m_listeners[i].OnOperationStarted(this);
            }
        }

        private void NotifyFinished()
        {
            for (int i = 0; i < m_listeners.Count; ++i)
            {
                m_listeners[i].OnOperationFinished(this);
            }
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
