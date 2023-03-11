using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billiard4Life
{
    public class Mediator
    {
        private static readonly Mediator instance = new Mediator();
        private IDictionary<string, List<Action<object>>> _mediator = new Dictionary<string, List<Action<object>>>();

        public static Mediator Instance
        {
            get { return instance; }
        }

        public void Subscribe(string token, Action<object> callback)
        {
            if (!_mediator.ContainsKey(token))
                _mediator[token] = new List<Action<object>>();

            _mediator[token].Add(callback);
        }

        public void NotifyColleagues(string token, object args)
        {
            if (_mediator.ContainsKey(token))
            {
                foreach (var callback in _mediator[token])
                {
                    callback(args);
                }
            }
        }
    }
}
