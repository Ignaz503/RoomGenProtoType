using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class InteractionFactory
{
    private static InteractionFactory _Instance;

    public static InteractionFactory Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = new InteractionFactory();
            return _Instance;
        }
    }

    Dictionary<Type, Type> BehaviourToContainerMapping = new Dictionary<Type, Type>();

    InteractionFactory()
    {
        Init();
    }

    //TODO DEFINE ATTRIBUTES
    void Init()
    {
        Assembly[] assembly = AppDomain.CurrentDomain.GetAssemblies();
        
        foreach(Assembly ass in assembly)
        {
            foreach(Type t in ass.GetTypes())
            {
                if (t.IsSubclassOf(typeof(Interaction)))
                {
                    Attribute att = t.GetCustomAttribute<InteractionAttribute>();
                    if(att != null)
                    {
                        InteractionAttribute iAtt = att as InteractionAttribute;

                        if (!iAtt.ContainerType.IsSubclassOf(typeof(BaseInteractionContainer)))
                        {
                                throw new Exception($"InteractionAttribute for class {t} needs to be a subclass of {typeof(BaseInteractionContainer)}");

                        }

                        BehaviourToContainerMapping.Add(t, iAtt.ContainerType);
                        Debug.Log($"Added mapping from {t} to {iAtt.ContainerType}");
                    }else {
                        throw new Exception($"{t} needs to define an {typeof(InteractionAttribute)} because it is a subclass of {typeof(Interaction)}");
                    }// att not null
                }// end if subclass
            }// end foreach type
        }// end foreach assembly
    }  

    public BaseInteractionContainer BuildInteractionContainer(string wanted_interaction, Func<Type> defaultBehaviour)
    {
        Type t = Type.GetType(wanted_interaction);

        if (t == null || !(t.IsSubclassOf(typeof(Interaction))) || !BehaviourToContainerMapping.ContainsKey(t))
        {
            //defaul behaviour it is
            Type def = defaultBehaviour();
            if (def == null)
                return null;
            else
            {
                Debug.Log(def);
                return Activator.CreateInstance(BehaviourToContainerMapping[def],new object[] { def }) as BaseInteractionContainer;
            }
        }
        else
        {
            return Activator.CreateInstance(BehaviourToContainerMapping[t], new object[] { t }) as BaseInteractionContainer;
        }
    }

}
