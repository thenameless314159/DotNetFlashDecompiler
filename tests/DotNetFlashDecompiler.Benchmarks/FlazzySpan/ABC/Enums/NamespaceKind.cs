﻿namespace FlazzySpan.ABC;

public enum NamespaceKind
{
    Private = 0x05,
    Namespace = 0x08,
    Package = 0x16,
    PackageInternal = 0x17,
    Protected = 0x18,
    Explicit = 0x19,
    StaticProtected = 0x1A
}