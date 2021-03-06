﻿using System;
using System.Collections.Generic;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// Contains info on the ships engines and fuel reserves.
    /// </summary>
    public class PropulsionDB : BaseDataBlob, ICreateViewmodel
    {
        public int MaximumSpeed { get; set; }
        public Vector4 CurrentVector { get; set; }
        public int TotalEnginePower { get; set; }
        public Dictionary<Guid, double> FuelUsePerKM { get; internal set; } = new Dictionary<Guid, double>();

        public PropulsionDB()
        {
        }

        public PropulsionDB(PropulsionDB propulsionDB)
        {
            MaximumSpeed = propulsionDB.MaximumSpeed;
            CurrentVector = propulsionDB.CurrentVector;
            TotalEnginePower = propulsionDB.TotalEnginePower;
            FuelUsePerKM = new Dictionary<Guid, double>(propulsionDB.FuelUsePerKM);
        }

        public override object Clone()
        {
            return new PropulsionDB(this);
        }

        public IDBViewmodel CreateVM(Game game, CommandReferences cmdRef)
        {
            return new TranslationMoveVM(game, cmdRef, this.OwningEntity);
        }
    }
}