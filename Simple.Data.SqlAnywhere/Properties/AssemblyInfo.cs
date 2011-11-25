using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if(DEBUG)
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Simple.Data.SqlAnywhere")]
[assembly: AssemblyDescription("SQL Anywhere add-in for ADO adapter.")]
[assembly: AssemblyCompany("Simple.Data.SqlAnywhere")]
[assembly: AssemblyProduct("Simple.Data.SqlAnywhere")]
[assembly: AssemblyCopyright("Copyright © Richard Hopton 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

[assembly: AssemblyVersion("0.11.3.1")]
[assembly: AssemblyFileVersion("0.11.3.1")]

