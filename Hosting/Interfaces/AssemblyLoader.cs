//==========================================================================================
//
//		MapSurfer.Utilities
//		Copyright (c) 2008-2014, MapSurfer.NET
//
//    Authors: Maxim Rylov
//
//==========================================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace MapSurfer.Web.Hosting
{
  internal static class AssemblyLoader
  {
    private static bool m_registered = false;
    private static string m_msnInstallPath;
    private static string m_version = string.Empty;
    private static AppDomain m_appDomain;

    private static RegistryKey GetRegistryKey(string keyName, RegistryHive regHive)
    {
      RegistryKey regKey = null;

      if (Environment.Is64BitOperatingSystem == true)
      {
        regKey = RegistryKey.OpenBaseKey(regHive, RegistryView.Registry64);
      }
      else
      {
        regKey = RegistryKey.OpenBaseKey(regHive, RegistryView.Registry32);
      }

      RegistryKey result = regKey.OpenSubKey(keyName);

      regKey.Dispose();
      regKey = null;

      return result;
    }

    private static string GetMSNInstallPath()
    {
      if (m_msnInstallPath == null)
      {
        string strVersion = m_version;
        string keyName = @"Software\MapSurfer.NET\" + strVersion;
        RegistryKey regKey = GetRegistryKey(keyName, RegistryHive.LocalMachine);

        string installPath = null;
        if (regKey != null)
        {
          installPath = (string)regKey.GetValue("InstallPath");

          // It might be that InstallPath is null as the previous version has been uninstalled or re-installed only for the current user. 
          if (string.IsNullOrEmpty(installPath))
          {
            regKey.Dispose();
            regKey = null;
          }
        }
        
        if (regKey == null)
        {
          regKey = GetRegistryKey(keyName, RegistryHive.CurrentUser);
        }

        if (regKey == null)
          throw new IOException(string.Format("Unable to determine installed version in the following path '{0}'.", keyName));

        installPath = (string)regKey.GetValue("InstallPath");

        if (string.IsNullOrEmpty(installPath))
          throw new IOException("InstallPath is null.");

        m_msnInstallPath = Path.Combine(installPath, "Core");

        regKey.Dispose();
        regKey = null;
      }

      return m_msnInstallPath;
    }

    public static void SetMSNInstallPath(string path)
    {
      if (!Directory.Exists(path))
        throw new DirectoryNotFoundException(path);

      m_msnInstallPath = path;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    public static void Register(string version)
    {
      Register(AppDomain.CurrentDomain, version);
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    public static void Register(AppDomain domain, string version)
    {
      if (m_registered)
        return;

      m_version = version;

      m_appDomain = domain;
      m_appDomain.AssemblyResolve += new ResolveEventHandler(Domain_AssemblyResolve);

      m_registered = true;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    public static void Unregister()
    {
      if (m_registered)
      {
        m_appDomain.AssemblyResolve -= new ResolveEventHandler(Domain_AssemblyResolve);
        m_appDomain = null;

        m_registered = false;
      }
    }

    public static Assembly LoadAssembly(string assemblyName)
    {
      string path = Path.Combine(GetMSNInstallPath(), assemblyName);
      AssemblyName name = AssemblyName.GetAssemblyName(path);
      Assembly assembly = Assembly.Load(name);

      return assembly;
    }

    private static Assembly Domain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
      if (args == null || string.IsNullOrEmpty(args.Name))
        return null;

      //Retrieve the list of referenced assemblies in an array of AssemblyName.
      Assembly objExecutingAssemblies = Assembly.GetExecutingAssembly();
      AssemblyName[] arrReferencedAssmbNames = objExecutingAssemblies.GetReferencedAssemblies();

      string dirAssemblies = GetMSNInstallPath();

      if (Directory.Exists(dirAssemblies))
      {
        string separator = Path.DirectorySeparatorChar.ToString();
        if (!dirAssemblies.EndsWith(separator))
          dirAssemblies += separator;

        string assemblyName = string.Empty;
        string fileName = string.Empty;
        string path = string.Empty;

        int p = args.Name.IndexOf(",");
        if (p > 0)
        {
          assemblyName = args.Name.Substring(0, p);
        }
        else
        {
          assemblyName = assemblyName.Replace(".dll", string.Empty);
        }

        fileName = assemblyName + ".dll";

        path = Path.Combine(dirAssemblies, fileName);
        if (File.Exists(path))
        {
          AssemblyName name = AssemblyName.GetAssemblyName(path);
          Assembly assembly = Assembly.Load(name);
          return assembly;
        }

        string[] dirs = Directory.GetDirectories(dirAssemblies, "*", SearchOption.AllDirectories);
        List<string> dirList = new List<string>();
        dirList.Add(dirAssemblies);
        dirList.AddRange(dirs);

        //Loop through the array of referenced assembly names.
        foreach (AssemblyName asmName in arrReferencedAssmbNames)
        {
          //Check for the assembly names that have raised the "AssemblyResolve" event.
          if (asmName.FullName.Substring(0, asmName.FullName.IndexOf(",")) == assemblyName)
          {
            if (dirList.Count > 0)
            {
              foreach (string strDirPath in dirList)
              {
                //Build the path of the assembly from where it has to be loaded.
                //The following line is probably the only line of code in this method you may need to modify:
                path = Path.Combine(strDirPath, fileName);

                if (File.Exists(path))
                {
                  //Load the assembly from the specified path.
                  AssemblyName name = AssemblyName.GetAssemblyName(path);
                  return Assembly.Load(name);
                }

                break;
              }
            }
            else
            {
              path = Path.Combine(dirAssemblies, fileName);

              if (File.Exists(path))
              {
                //Load the assembly from the specified path.
                AssemblyName name = AssemblyName.GetAssemblyName(path);
                return Assembly.Load(name);
              }
            }
          }
        }
      }

      return null;
    }
  }
}
