﻿#region Copyright/License
/* 
 *Copyright© 2017 Daniel Phelps
    This file is part of Pulsar4x.

    Pulsar4x is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Pulsar4x is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Pulsar4x.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion
using System;
using System.Collections.Generic;

namespace Pulsar4X.ECSLib
{
    public class ColonyMinesDB : BaseDataBlob
    {
        public ObservableDictionary<Guid, int> MiningRate { get; set; } = new ObservableDictionary<Guid, int>();

        public ColonyMinesDB()
        {
            MiningRate.CollectionChanged += (sender, args) => OnSubCollectionChanged(nameof(MiningRate), args);
        }

        public ColonyMinesDB(IDictionary<Guid, int> miningRate) : this()
        {
            MiningRate.Merge(miningRate);
        }

        public ColonyMinesDB(ColonyMinesDB db) : this(db.MiningRate) { }


        public override object Clone()
        {
            return new ColonyMinesDB(this);
        }
    }
}