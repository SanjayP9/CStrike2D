// Author: Mark Voong
// File Name: Entity.cs
// Project: CStrike2D
// Date Created: Dec 6th 2015
// Date Modified:
// Description: An object that is able to interact with the world
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cstrike2d
{
    abstract class Entity
    {
        public abstract int DrawOrder { get; protected set; }
    }
}
