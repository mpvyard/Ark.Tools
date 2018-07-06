﻿using NodaTime;

namespace Ark.Tools.ResourceWatcher
{
    public interface IResourceState
    {
        Instant RetrievedAt { get; }
        /// <summary>
        /// Checksum provided back to the data retriver to avoid processing of the resource (i.e. parsing). 
        /// It's checked even in case of new IResourceInfo.Modified in case of spurious modifications.
        /// </summary>
        string CheckSum { get; }
    }
}
