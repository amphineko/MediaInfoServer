﻿namespace PodcastCore.LibMediaInfo
{
    public enum ReadStatus
    {
        None = 0x00,
        Accepted = 0x01,
        Filled = 0x02,
        Updated = 0x04,
        Finalized = 0x08
    }
}