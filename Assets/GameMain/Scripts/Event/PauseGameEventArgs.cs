using GameFramework.Event;
using GameFramework;

namespace Game
{
    public class PauseGameEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(PauseGameEventArgs).GetHashCode();

        public PauseGameEventArgs()
        {
            IsPause = false;
        }

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public bool IsPause
        {
            get;
            private set;
        }

        public static PauseGameEventArgs Create(bool IsPause)
        {
            PauseGameEventArgs pauseGameEventArgs = ReferencePool.Acquire<PauseGameEventArgs>();
            pauseGameEventArgs.IsPause = IsPause;
            return pauseGameEventArgs;
        }

        public override void Clear()
        {
            IsPause = false;
        }
    }

}

