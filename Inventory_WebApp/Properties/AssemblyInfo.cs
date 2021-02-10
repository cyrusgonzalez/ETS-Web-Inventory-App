using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Inventory_WebApp")]
[assembly: AssemblyDescription("A lightweight web application designed for inventory management for the ETS, Dept of Engineering, Colorado State University by Sanket Mehrotra")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Engineering Technical Services, Department of Engineering, Colorado State University")]
[assembly: AssemblyProduct("Inventory_WebApp")]
[assembly: AssemblyCopyright("Copyright © Colorado State University Engineering Technical Services 2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("acc1f4c2-764c-4420-84c3-9c0ff630cbbc")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]


//This line tells log4net where it's config file is.
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]