﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{

    public static class FactionFactory
    {
        /*
         *Stuff a faction needs to know:
         *name (nameDB)
         *password (AuthDB)
         *researched tech. (techDB)
         *
         *Owned Entites
         *
         *Sensor Contacts - these will be owned entites anyway.
         *  -System Bodies
         *  -Non Owned Entites
         *      -Colones
         *      -Ships
         * 
         *Sensor Types
         *  - Grav, ie detecting anomalies in paths of known objects. - slow, but will find large dark planets. 
         *  - Passive EM Spectrum:
         *      - Emited visable light (suns)
         *      - Reflected visable light (planets, moons)
         *      - Emitted IR (colonies, ship drives)
         *      - Reflected IR
         *      - Comms emmisions (colonies, ships)
         *  - Active EM:
         *      - Emmitting EM and looking for an echo. (radar) 
         * 
         * Owned Enties and Sensor Contacts need to be broken down by system. 
         * 
         * 
         */


        public static Entity CreateFaction(Game game, string factionName)
        {
            var name = new NameDB(factionName);

            var blobs = new List<BaseDataBlob> { 
                name, 
                new FactionInfoDB(), 
                new FactionAbilitiesDB(), 
                new FactionTechDB(game.StaticData.Techs.Values.ToList()), 
                new FactionOwnerDB(),
            };
            var factionEntity = new Entity(game.GlobalManager, blobs);

            // Add this faction to the SM's access list.
            game.SpaceMaster.SetAccess(factionEntity, AccessRole.SM);
            name.SetName(factionEntity, factionName);
            return factionEntity;
        }


        //TODO: #Cleanup #Deadcode. Curently not using this anywhere, I think. have I discarded the idea? 
        /// <summary>
        /// The idea of this was to have an entity for system specific faction info. would contain a list of owned entities in that system, etc. 
        /// 
        /// </summary>
        /// <returns>The system faction entity.</returns>
        /// <param name="game">Game.</param>
        /// <param name="faction">Faction.</param>
        /// <param name="starSystem">Star system.</param>
        public static Entity CreateSystemFactionEntity(Game game, Entity faction, StarSystem starSystem)
        {
            var blobs = new List<BaseDataBlob>
            {
                faction.GetDataBlob<NameDB>(),//it apears that two different entites can share a datablob. this could be useful, 
                //but could also be tricky if a procesor does an action on all entites with datablob X

                new FactionSystemInfoDB(starSystem),
            };
            var systemFactionEntity = Entity.Create(starSystem, blobs);
            new OwnedDB(faction, systemFactionEntity);
            return systemFactionEntity;
        }

        public static Entity CreatePlayerFaction(Game game, Player owningPlayer, string factionName)
        {
            Entity faction = CreateFaction(game, factionName);


            if (!Equals(owningPlayer, game.SpaceMaster))
            {
                owningPlayer.SetAccess(faction, AccessRole.Owner);
            }

            return faction;
        }


    }
    public class FactionSystemInfoDB : BaseDataBlob
    {
        internal StarSystem StarSystem;
        internal HashSet<Entity> OwnedEntitesInSystem = new HashSet<Entity>();
        internal HashSet<Entity> KnownSystemBodies = new HashSet<Entity>();

        public FactionSystemInfoDB() { }

        public FactionSystemInfoDB(StarSystem starSystem)
        {
            this.StarSystem = starSystem;
            //TODO move this to a processor and require sensors of some kind. 
            //will also need to figure out how much a faction knows about the body. 
            foreach (var body in starSystem.GetAllEntitiesWithDataBlob<SystemBodyInfoDB>())
            {
                KnownSystemBodies.Add(body);
            }
        }

        public FactionSystemInfoDB(FactionSystemInfoDB db)
        {
            StarSystem = db.StarSystem;
            OwnedEntitesInSystem = new HashSet<Entity>(db.OwnedEntitesInSystem);
            KnownSystemBodies = new HashSet<Entity>(db.KnownSystemBodies);
        }

        public override object Clone()
        {
            return new FactionSystemInfoDB(this);
        }
    }
}