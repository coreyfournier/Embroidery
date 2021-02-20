using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Embroidery.Client.ViewModels
{
    public class ObservableT<T> : IObservable<T>, IDisposable
    {
        private IObserver<T> _observer;
        private T _value;

        public T Value {
            get { 
                return _value; 
            }
            set {   
                _value = value;

                
                if (_observer != null)
                    _observer.OnNext(_value);
            }
        }

        public void Dispose()
        {
            
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observer = observer;

            return this;
        }
    }
}
