using Extensions;
using System;
using System.Collections.Generic;

namespace ServerApp.Models
{
    public class Graph : BindableBase
    {
        private KeyValuePair<DateTime, double>[] _dots;

        public KeyValuePair<DateTime, double>[] Dots
        {
            get => _dots;
            set => SetProperty(ref _dots, value);
        }

        public Graph()
        {
            Dots = new KeyValuePair<DateTime, double>[0];
        }
    }
}