﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minista.Classes
{
    public class MinistaSettings
    {
        public HeaderPosition HeaderPosition { get; set; } = HeaderPosition.Top;
        public bool GhostMode { get; set; } = false;
        public bool AskedAboutPosition { get; set; } = false;
        public bool ElementSound { get; set; } = false;
        public bool RemoveAds { get; set; } = true;
        public bool DownloadLocationChanged { get; set; } = false;
        public double LockControlX { get; set; } = 0;
        public double LockControlY { get; set; } = 120;
    } 
    public enum HeaderPosition
    {
        Top,
        Bottom
    }
}
