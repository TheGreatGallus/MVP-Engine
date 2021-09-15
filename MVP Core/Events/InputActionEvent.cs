using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVP_Core.Events
{
    public class InputActionEvent
    {
        // TODO: Move to XML config
        public enum Actions
        {
            UP,
            LEFT,
            DOWN,
            RIGHT,
            JUMP,
            SHOOT,
            RUN,
            TOGGLE,
            AIM,
            PAUSE
        }
        public enum States
        {
            NULL,
            START,
            HELD,
            STOP
        }

        public Actions action;
        public States state;

        public InputActionEvent(Actions action)
        {
            this.action = action;
            this.state = States.NULL;
        }

        public void Process()
        {

        }

        public void SetState(States state)
        {
            this.state = state;
        }
    }
}
