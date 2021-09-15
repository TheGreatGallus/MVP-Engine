using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MVP_Core.Managers
{
    class EffectManager : Manager<Effect>
    {
        private static readonly Lazy<EffectManager> lazy =
            new Lazy<EffectManager>(() => new EffectManager());

        public static EffectManager Instance { get { return lazy.Value; } }

        private string currentEffectName;
        private bool IsDeactivated = false;

        private EffectManager()
        {
            Initialize();
        }

        public void ApplyEffect(string effectName)
        {
            if (!IsDeactivated)
            {
                currentEffectName = effectName;
                GetItem(effectName).CurrentTechnique.Passes[0].Apply();
            }
        }

        public bool ApplyParameters(string[] names, Type[] types, object[] parameters)
        {
            if (!IsDeactivated)
            {
                if (names.Length != types.Length || types.Length != parameters.Length)
                    return false;
                try
                {
                    Effect current = GetItem(currentEffectName);
                    for (int i = 0; i < names.Length; i++)
                    {
                        current.GetType().GetMethod("SetValue", new Type[] { types[i] }).Invoke(current, new object[] { parameters });
                    }
                }
                catch (Exception e)
                {
                    return false;
                }
                return true;
            }
            return true;
        }

        public bool ApplyParameters(List<Tuple<string, Type, object>> paramters)
        {
            if (!IsDeactivated)
            {
                try
                {
                    Effect current = GetItem(currentEffectName);
                    for (int i = 0; i < paramters.Count; i++)
                    {
                        Tuple<string, Type, object> currentTuple = paramters[i];
                        // GOOD CODE (WORKS CORRECTLY), BAD IDEA (TOO SLOW)
                        //EffectParameter currentParameter = current.Parameters[currentTuple.Item1];
                        //currentParameter.GetType().GetMethod("SetValue", new Type[] { currentTuple.Item2 }).Invoke(currentParameter, new object[] { currentTuple.Item3 });
                        if (currentTuple.Item2 == typeof(bool))
                        {
                            current.Parameters[currentTuple.Item1].SetValue((bool)currentTuple.Item3);
                        }
                        else if (currentTuple.Item2 == typeof(float))
                        {
                            //current.Parameters[currentTuple.Item1].SetValue((float)currentTuple.Item3);
                            current.Parameters[currentTuple.Item1].SetValue((float)Convert.ToDouble(currentTuple.Item3));
                        }
                        else if (currentTuple.Item2 == typeof(float[]))
                        {
                            current.Parameters[currentTuple.Item1].SetValue((float[])currentTuple.Item3);
                        }
                        else if (currentTuple.Item2 == typeof(int))
                        {
                            current.Parameters[currentTuple.Item1].SetValue((int)currentTuple.Item3);
                        }
                        else if (currentTuple.Item2 == typeof(Matrix))
                        {
                            current.Parameters[currentTuple.Item1].SetValue((Matrix)currentTuple.Item3);
                        }
                        else if (currentTuple.Item2 == typeof(Matrix[]))
                        {
                            current.Parameters[currentTuple.Item1].SetValue((Matrix[])currentTuple.Item3);
                        }
                        else if (currentTuple.Item2 == typeof(Quaternion))
                        {
                            current.Parameters[currentTuple.Item1].SetValue((Quaternion)currentTuple.Item3);
                        }
                        else if (currentTuple.Item2 == typeof(Texture))
                        {
                            current.Parameters[currentTuple.Item1].SetValue((Texture)currentTuple.Item3);
                        }
                        else if (currentTuple.Item2 == typeof(Vector2))
                        {
                            current.Parameters[currentTuple.Item1].SetValue((Vector2)currentTuple.Item3);
                        }
                        else if (currentTuple.Item2 == typeof(Vector2[]))
                        {
                            current.Parameters[currentTuple.Item1].SetValue((Vector2[])currentTuple.Item3);
                        }
                        else if (currentTuple.Item2 == typeof(Vector3))
                        {
                            current.Parameters[currentTuple.Item1].SetValue((Vector3)currentTuple.Item3);
                        }
                        else if (currentTuple.Item2 == typeof(Vector3[]))
                        {
                            current.Parameters[currentTuple.Item1].SetValue((Vector3[])currentTuple.Item3);
                        }
                        else if (currentTuple.Item2 == typeof(Vector4))
                        {
                            current.Parameters[currentTuple.Item1].SetValue((Vector4)currentTuple.Item3);
                        }
                        else if (currentTuple.Item2 == typeof(Vector4[]))
                        {
                            current.Parameters[currentTuple.Item1].SetValue((Vector4[])currentTuple.Item3);
                        }
                    }
                }
                catch (Exception e)
                {
                    return false;
                }
                return true;
            }
            return true;
        }

        public void DeactivateEffects()
        {
            IsDeactivated = true;
        }

        public void ActivateEffects()
        {
            IsDeactivated = false;
        }
    }
}
