﻿using FCG.Core.Services.Interfaces;

namespace FCG.Core.Services;

public class CorrelationIdGenerator : ICorrelationIdGenerator
{
    private static string _correlationId;

    public string Get() => _correlationId;

    public void Set(string correlationId) => _correlationId = correlationId;
}
