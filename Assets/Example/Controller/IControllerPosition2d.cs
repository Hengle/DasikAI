﻿using DasikAI.Common.Controller;
using UnityEngine;

namespace DasikAI.Example.Controller
{
    public interface IControllerPosition2d:IAgentController
    {
        Vector2 Position2d { get; set; }
    }
}