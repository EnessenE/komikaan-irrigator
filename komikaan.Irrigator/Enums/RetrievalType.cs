﻿namespace komikaan.Irrigator.Enums;

public enum RetrievalType
{
    /// <summary>
    ///  Requires a form of HTTP or HTTPS call
    /// </summary>
    REST,
    /// <summary>
    /// A zip file locally stored, not to be used in prod
    /// </summary>
    LOCAL
}