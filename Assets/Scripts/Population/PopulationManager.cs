using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Population
{
    public class PopulationManager
    {
        private static PopulationManager instance;
        public static PopulationManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new PopulationManager();
                return instance;
            }
        }

        public HashSet<Person> IdlePeople { get; private set; }
        public HashSet<Person> TreeChoppers { get; private set; }
        public HashSet<Person> StoneMiners { get; private set; }

        //////DICTIONARY WITH PRIORITIES FOR EACH JOB? KEY JOB, VALUE PRIORITY?//////

        private PopulationManager()
        {
            IdlePeople = new HashSet<Person>();
            TreeChoppers = new HashSet<Person>();
            StoneMiners = new HashSet<Person>();
        }


        public void RegisterPerson(Person person)
        {
            switch (person.PersonState)
            {
                case Person.PersonStates.Idling:
                    IdlePeople.Add(person);
                    break;
                case Person.PersonStates.ChoppingTree:
                    TreeChoppers.Add(person);
                    break;
                case Person.PersonStates.MiningStone:
                    StoneMiners.Add(person);
                    break;
                default:
                    break;
            }
        }

        public void RemovePerson(Person person)
        {
            switch (person.PersonState)
            {
                case Person.PersonStates.Idling:
                    IdlePeople.Remove(person);
                    break;
                case Person.PersonStates.ChoppingTree:
                    TreeChoppers.Remove(person);
                    break;
                case Person.PersonStates.MiningStone:
                    StoneMiners.Remove(person);
                    break;
                default:
                    break;
            }
        }


    }   
}
