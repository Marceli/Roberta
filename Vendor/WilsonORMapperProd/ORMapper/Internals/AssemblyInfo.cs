//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

// General Information
#if DEBUG && DOTNETV2
[assembly: AssemblyTitle("Wilson.ORMapper:Debug.DotNetV2")]
#elif DEBUG
[assembly: AssemblyTitle("Wilson.ORMapper:Debug")]
#elif DEMO && DOTNETV2
[assembly: AssemblyTitle("Wilson.ORMapper:Development.DotNetV2")]
#elif DEMO
[assembly: AssemblyTitle("Wilson.ORMapper:Development")]
#elif DOTNETV2
[assembly: AssemblyTitle("Wilson.ORMapper:Production.DotNetV2")]
#else
[assembly: AssemblyTitle("Wilson.ORMapper:Production")]
#endif
[assembly: AssemblyDescription("Paul@WilsonDotNet.com")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("www.WilsonDotNet.com")]
[assembly: AssemblyProduct("WilsonORMapper")]
[assembly: AssemblyCopyright("©2003-2006 Paul Wilson")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Version Information
[assembly: AssemblyVersion("4.2.2.0")]
[assembly: CLSCompliant(true)]

// Signing Information
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]
