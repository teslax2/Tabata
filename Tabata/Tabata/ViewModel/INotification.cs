using System;
using System.Collections.Generic;
using System.Text;

namespace Tabata.ViewModel
{
    public interface INotification
    {
        void Create(string message);
        void Hide();
        void Update(string message);
    }
}
