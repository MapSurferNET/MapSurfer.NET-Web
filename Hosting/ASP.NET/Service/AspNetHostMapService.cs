//==========================================================================================
//
//		MapSurfer.Web.Hosting
//		Copyright (c) 2008-2015, MapSurfer.NET
//
//    Authors: Maxim Rylov
//
//==========================================================================================
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;

using MapSurfer.Web.Interfaces.AspNet;

[assembly: PreApplicationStartMethod(typeof(MapSurfer.Web.Hosting.AspNet.AspNetHostMapService), "RegisterModules")]

namespace MapSurfer.Web.Hosting.AspNet
{
  # region ************** Configuration Section **************

  public class DynamicHttpModulesConfiguration : ConfigurationSection
  {
    public DynamicHttpModulesConfiguration()
    { }

    [ConfigurationProperty("Modules")]
    public DynamicHttpModuleCollection Modules
    {

      get { return ((DynamicHttpModuleCollection)(base["Modules"])); }

    }
  }

  public class DynamicHttpModuleElement : ConfigurationElement
  {
    public DynamicHttpModuleElement()
      : this(string.Empty, string.Empty)
    {
    }

    public DynamicHttpModuleElement(string name, string type)
    {
      Name = name;
      Type = type;
    }

    [ConfigurationProperty("name", DefaultValue = "", IsRequired = true, IsKey = true)]
    public string Name
    {
      get { return (string)this["name"]; }
      set { this["name"] = value; }
    }

    [ConfigurationProperty("type", DefaultValue = "", IsRequired = true, IsKey = false)]
    public string Type
    {
      get { return (string)this["type"]; }
      set { this["type"] = value; }
    }

    [ConfigurationProperty("path", DefaultValue = "", IsRequired = true, IsKey = false)]
    public string Path
    {
      get { return (string)this["path"]; }
      set { this["path"] = value; }
    }
  }

  public class DynamicHttpModuleCollection : ConfigurationElementCollection
  {
    public DynamicHttpModuleElement this[int index]
    {
      get
      {
        return base.BaseGet(index) as DynamicHttpModuleElement;
      }
      set
      {
        if (base.BaseGet(index) != null)
        {
          base.BaseRemoveAt(index);
        }
        this.BaseAdd(index, value);
      }
    }

    protected override ConfigurationElement CreateNewElement()
    {
      return new DynamicHttpModuleElement();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return ((DynamicHttpModuleElement)element).Name;
    }
  }

  #endregion

  /// <summary>
  /// Summary description for AspNetHostMapService
  /// </summary>
  public class AspNetHostMapService : AbstractMapService
  {
    private static readonly Action<Type> Fx45RegisterModuleDelegate = GetFx45RegisterModuleDelegate();

    public AspNetHostMapService()
    {

    }

    private static Action<Type> GetFx45RegisterModuleDelegate()
    {
      MethodInfo method = null;

      try
      {
        method = typeof(HttpApplication).GetMethod("RegisterModule", BindingFlags.Static | BindingFlags.Public, null, new Type[]
	      {
		      typeof(Type)
	      }, null);

        if (!(method != null))
        {
          return null;
        }
      }
      catch (Exception ex)
      {
        throw new Exception("Unable to find the method RegisterModule in HttpApplication.", ex);
      }

      return (Action<Type>)Delegate.CreateDelegate(typeof(Action<Type>), method);
    }

    public static void RegisterModules()
    {
      CheckInstallPath();

      string strVersion = (string)ConfigurationManager.AppSettings["MapSurfer.Version"];
      AssemblyLoader.Register(strVersion);

      try
      {
        DynamicHttpModulesConfiguration section = (DynamicHttpModulesConfiguration)ConfigurationManager.GetSection("DynamicHttpModules");

        if (section != null)
        {
          foreach (DynamicHttpModuleElement module in section.Modules)
          {
            string path = module.Path;
            Assembly assembly = null;

            if (!string.IsNullOrEmpty(path) || !Path.IsPathRooted(path))
            {
              assembly = AssemblyLoader.LoadAssembly(path);
            }
            else
            {
              if (File.Exists(path))
              {
                AssemblyName name = AssemblyName.GetAssemblyName(path);
                assembly = Assembly.Load(name);
              }
            }

            if (assembly == null)
              continue;

            string typeName = module.Type;

            if (typeName != null)
            {
              int index = typeName.IndexOf(",");
              if (index > 0)
                typeName = typeName.Substring(0, index);

              Type type = assembly.GetType(typeName, true);

              if (type == null)
              {
                System.Diagnostics.Debug.WriteLine("Unable to find the type " + typeName);
              }
              else
              {
                RegisterModule(type);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine(ex.Message + "StackTrace:" + ex.StackTrace);
        throw ex;
      }

      AssemblyLoader.Unregister();
    }

    private static void RegisterModule(Type type)
    {
      try
      {
        if (Fx45RegisterModuleDelegate != null)
        {
          Fx45RegisterModuleDelegate(type);
        }
        else
        {
#if !MONO && !DEBUG
      //    Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(type);
#endif
        }
      }
      catch (Exception ex)
      {
        throw new Exception("Unable to register type " + type.FullName, ex);
      }
    }
  }
}