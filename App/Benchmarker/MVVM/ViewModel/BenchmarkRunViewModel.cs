﻿using Benchmarker.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Benchmarker.MVVM.ViewModel
{
    internal class BenchmarkRunViewModel : ObservableObject
    {
        public RelayCommand SwitchView { get; set; }
        public string appName { get; set; }
        private string _currentCPU { get; set; }
        private OpenFileDialog _file { get; set; }
        private DispatcherTimer _timer;
        private Process _process;
        private DateTime prevCheck;
        private TimeSpan prevTotalCPUTime;

        public OpenFileDialog File
        {
            get { return _file; }
            set
            {
                _file = value;
                appName = _file.SafeFileName;

                _process = new Process();
                _process.StartInfo.FileName = _file.FileName;
                _process.Start();
                prevCheck = _process.StartTime;
                prevTotalCPUTime = new TimeSpan(0);

                _timer = new System.Windows.Threading.DispatcherTimer();
                _timer.Tick += new EventHandler(dispatcherTimer_Tick);
                _timer.Interval = new TimeSpan(0, 0, 1);
                _timer.Start();

                OnPropertyChanged();
            }
        }

        public string currentCPU
        {
            get { return _currentCPU; }
            set
            {
                _currentCPU = value;
                OnPropertyChanged();
            }
        }

        public BenchmarkRunViewModel(RelayCommand switchView)
        {
            appName = "INSTANTIATING";
            SwitchView = new RelayCommand(o =>
            {
                if (_process != null && !_process.HasExited)
                {
                    _process.Kill();
                    _timer.Stop();
                }
                switchView.Execute(this);
            });
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var newTotalCPUTime = _process.TotalProcessorTime;
            TimeSpan elapsed = DateTime.Now - prevCheck;
            if (elapsed.TotalSeconds > 0)
            {
                TimeSpan timeThisCheck = (newTotalCPUTime - prevTotalCPUTime);
                double CPUusage = (double)timeThisCheck.Ticks / elapsed.Ticks;
                currentCPU = String.Format("{0:0.00}%", CPUusage*100);
            }
            prevCheck = DateTime.Now;
            prevTotalCPUTime = newTotalCPUTime;
        }
    }
}