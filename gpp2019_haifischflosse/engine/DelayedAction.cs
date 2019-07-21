using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaGeometry;

namespace gpp2019_haifischflosse
{
    public class DelayedAction : IFormatProvider, IComparable
    {
        public DelayedAction(Action action, double delay)
        {
            TimeRemaining = delay;
            Action = action;
            Delay = delay;
        }

        public Action Action { get; private set; }
        public double Delay { get; private set; }
        public double TimeRemaining { get; private set; }

        public bool Update(double deltaTimeSec)
        {
            TimeRemaining -= deltaTimeSec;

            if (TimeRemaining <= 0)
            {
                Action();
                TimeRemaining = Delay;
                return true;
            }
            return false;
        }

        public int CompareTo(Object o)
        {
            if (o is DelayedAction d)
            {
                return Delay.CompareTo(d.Delay);
            }
            throw new ArgumentException("o is not an DelayedAction object.");
        }

        public object GetFormat(Type t)
        {
            if (t.Equals(this.GetType()))
                return this;
            return null;
        }
    }
}
