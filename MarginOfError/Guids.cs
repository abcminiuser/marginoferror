// Guids.cs
// MUST match guids.h
using System;

namespace FourWalledCubicle.MarginOfError
{
    static class GuidList
    {
        public const string guidMarginOfErrorPkgString = "d2d447c3-c682-4954-b6ae-899463dd1a63";
        public const string guidMarginOfErrorCmdSetString = "c8699cd0-7ce0-4739-bae8-74466a6b304e";

        public static readonly Guid guidMarginOfErrorCmdSet = new Guid(guidMarginOfErrorCmdSetString);
    };
}