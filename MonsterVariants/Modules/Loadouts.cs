using EntityStates;
using RoR2;
using System;
using System.Reflection;
using System.Collections.Generic;
using MonoMod.RuntimeDetour;
using RoR2.Skills;

namespace MonsterVariants.Modules
{
    internal static class Loadouts
    {
        internal static List<Type> entityStates = new List<Type>();
        internal static List<SkillDef> skillDefs = new List<SkillDef>();

        private static Hook set_stateTypeHook;
        private static Hook set_typeNameHook;
        private static readonly BindingFlags allFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;
        private delegate void set_stateTypeDelegate(ref SerializableEntityStateType self, Type value);
        private delegate void set_typeNameDelegate(ref SerializableEntityStateType self, String value);

        internal static void AddSkill(Type t)
        {
            entityStates.Add(t);
        }

        internal static void AddSkillDef(SkillDef t)
        {
            skillDefs.Add(t);
        }

        internal static void FixStates()
        {
            // fixing a vanilla bug
            Type type = typeof(SerializableEntityStateType);
            HookConfig cfg = default;
            cfg.Priority = Int32.MinValue;
            set_stateTypeHook = new Hook(type.GetMethod("set_stateType", allFlags), new set_stateTypeDelegate(SetStateTypeHook), cfg);
            set_typeNameHook = new Hook(type.GetMethod("set_typeName", allFlags), new set_typeNameDelegate(SetTypeName), cfg);
            //
        }

        // ignore this
        private static void SetStateTypeHook(ref this SerializableEntityStateType self, Type value)
        {
            self._typeName = value.AssemblyQualifiedName;
        }

        private static void SetTypeName(ref this SerializableEntityStateType self, String value)
        {
            Type t = GetTypeFromName(value);
            if (t != null)
            {
                self.SetStateTypeHook(t);
            }
        }

        private static Type GetTypeFromName(String name)
        {
            Type[] types = EntityStateCatalog.stateIndexToType;
            return Type.GetType(name);
        }
    }
}
