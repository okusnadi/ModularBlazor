﻿using MainApplication.Serrvices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace MainApplication.Services
{
    public class ModuleManager
    {
        private Dictionary<PluginLoadContext, Assembly> _loadContexts = new Dictionary<PluginLoadContext, Assembly>();

        public event Action<IEnumerable<Assembly>> OnModulesLoaded;

        public bool Loaded { get; private set; }

        public void LoadModules()
        {
            var modules = new List<string>
            {
                @"Module1\bin\debug\netstandard2.0\Module1.dll",
                @"Module2\bin\debug\netstandard2.0\Module2.dll"
            };

            foreach(var modulePath in modules)
            {
                LoadModule(modulePath, out var pluginLoadContext, out var assembly);
                _loadContexts.Add(pluginLoadContext, assembly);
            }

            OnModulesLoaded?.Invoke(_loadContexts.Values);
            Loaded = true;
        }

        public void UnloadModules()
        {
            foreach(var pluginLoadContext in _loadContexts)
            {
                pluginLoadContext.Key.Unload();
            }

            OnModulesLoaded?.Invoke(null);
            _loadContexts.Clear();
            Loaded = false;
        }

        private void LoadModule(string modulePath, out PluginLoadContext pluginLoadContext, out Assembly assembly)
        {
            string root = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(
                                Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

            string pluginLocation = Path.GetFullPath(Path.Combine(root, modulePath.Replace('\\', Path.DirectorySeparatorChar)));

            pluginLoadContext = new PluginLoadContext(pluginLocation);
            assembly = pluginLoadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }
    }
}